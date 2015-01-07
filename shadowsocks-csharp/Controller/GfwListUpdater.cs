﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.Controller
{
    public class GfwListUpdater
    {
        private const string GFWLIST_URL = "https://autoproxy-gfwlist.googlecode.com/svn/trunk/gfwlist.txt";

        private const int EXPIRE_HOURS = 6;

        public IWebProxy proxy = null;

        public bool useSystemProxy = true;

        public class GfwListChangedArgs : EventArgs
        {
            public string[] GfwList { get; set; }
        }

        public event EventHandler<GfwListChangedArgs> GfwListChanged; 

        private bool running = false;
        private bool closed = false;
        private int jobId = 0;
        DateTime lastUpdateTimeUtc;
        string lastUpdateMd5;

        private object locker = new object();

        public GfwListUpdater()
        {
        }

        ~GfwListUpdater()
        {
            Stop();
        }

        public void Start()
        {
            lock (locker)
            {
                if (running)
                    return;
                running = true;
                closed = false;
                jobId++;
                new Thread(new ParameterizedThreadStart(UpdateJob)).Start(jobId);
            }
        }

        public void Stop()
        {
            lock(locker)
            {
                closed = true;
                running = false;
                jobId++;
            }
        }

        public void ScheduleUpdateTime(int delaySeconds)
        {
            lock(locker)
            {
                lastUpdateTimeUtc = DateTime.UtcNow.AddHours(-1 * EXPIRE_HOURS).AddSeconds(delaySeconds);
            }
        }

        private string DownloadGfwListFile()
        {
            try
            {
                WebClient http = new WebClient();
                http.Proxy = useSystemProxy ? WebRequest.GetSystemWebProxy() : proxy;
                return http.DownloadString(new Uri(GFWLIST_URL));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        private bool IsExpire()
        {
            lock (locker)
            {
                TimeSpan ts = DateTime.UtcNow - lastUpdateTimeUtc;
                bool expire = ((int)ts.TotalHours) >= EXPIRE_HOURS;
                if (expire)
                    lastUpdateTimeUtc = DateTime.UtcNow;
                return expire;
            }
        }

        private bool IsJobStop(int currentJobId)
        {
            lock (locker)
            {
                if (!running || closed || currentJobId != this.jobId)
                    return true;
            }
            return false;
        }

        private bool IsGfwListChanged(string content)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(content);
            byte[] md5Bytes = MD5.Create().ComputeHash(inputBytes);
            string md5 = "";
            for (int i = 0; i < md5Bytes.Length; i++)
                md5 += md5Bytes[i].ToString("x").PadLeft(2, '0');
            if (md5 == lastUpdateMd5)
                return false;
            lastUpdateMd5 = md5;
            return true;
        }

        private void ParseGfwList(string response)
        {
            if (!IsGfwListChanged(response))
                return;
            if (GfwListChanged != null)
            {
                try
                {
                    Parser parser = new Parser(response);
                    GfwListChangedArgs args = new GfwListChangedArgs
                    {
                        GfwList = parser.GetReducedDomains()
                    };
                    GfwListChanged(this, args);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void UpdateJob(object state)
        {
            int currentJobId = (int)state;
            int retryTimes = 3;
            while (!IsJobStop(currentJobId))
            {
                if (IsExpire())
                {
                    string response = DownloadGfwListFile();
                    if (response != null)
                    {
                        ParseGfwList(response);
                    }
                    else if (retryTimes > 0)
                    {
                        ScheduleUpdateTime(30); /*Delay 30 seconds to retry*/
                        retryTimes--;
                    }
                    else
                    {
                        retryTimes = 3; /* reset retry times, and wait next update time. */
                    }
                }

                Thread.Sleep(1000);
            }
        }

        class Parser
        {
            public string Content { get; private set; }

            public Parser(string response)
            {
                byte[] bytes = Convert.FromBase64String(response);
                this.Content = Encoding.ASCII.GetString(bytes);
            }

            public string[] GetLines()
            {
                return Content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); 
            }
            
            /* refer https://github.com/clowwindy/gfwlist2pac/blob/master/gfwlist2pac/main.py */
            public string[] GetDomains()
            {
                string[] lines = GetLines();
                List<string> domains = new List<string>(lines.Length);
                for(int i =0;i < lines.Length;i++)
                {
                    string line = lines[i];
                    if (line.IndexOf(".*") >= 0)
                        continue;
                    else if (line.IndexOf("*") >= 0)
                        line = line.Replace("*", "/");
                    if (line.StartsWith("||"))
                        line = line.Substring(2);
                    else if (line.StartsWith("|"))
                        line = line.Substring(1);
                    else if (line.StartsWith("."))
                        line = line.Substring(1);
                    if (line.StartsWith("!"))
                        continue;
                    else if (line.StartsWith("["))
                        continue;
                    else if (line.StartsWith("@"))
                        continue; /*ignore white list*/
                    domains.Add(line);
                }
                return domains.ToArray();
            }

            /* refer https://github.com/clowwindy/gfwlist2pac/blob/master/gfwlist2pac/main.py */
            public string[] GetReducedDomains()
            {
                string[] domains = GetDomains();
                List<string> new_domains = new List<string>(domains.Length);
                IDictionary<string, string> tld_dic = GetTldDictionary();

                foreach(string domain in domains)
                {
                    string last_root_domain = null;
                    int pos;
                    pos = domain.LastIndexOf('.');
                    last_root_domain = domain.Substring(pos + 1);
                    if (!tld_dic.ContainsKey(last_root_domain))
                        continue;
                    while(pos > 0)
                    {
                        pos = domain.LastIndexOf('.', pos - 1);
                        last_root_domain = domain.Substring(pos + 1);
                        if (tld_dic.ContainsKey(last_root_domain))
                            continue;
                        else
                            break;
                    }
                    if (last_root_domain != null)
                        new_domains.Add(last_root_domain);
                }

                return new_domains.ToArray();
            }

            private string[] GetTlds()
            {
                string[] tlds = null;
                byte[] pacGZ = Resources.tld_txt;
                byte[] buffer = new byte[1024]; 
                int n;
                using(MemoryStream sb = new MemoryStream())
                {
                    using (GZipStream input = new GZipStream(new MemoryStream(pacGZ),
                        CompressionMode.Decompress, false))
                    {
                        while ((n = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            sb.Write(buffer, 0, n);
                        }
                    }
                    tlds = System.Text.Encoding.UTF8.GetString(sb.ToArray())
                        .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                return tlds;
            }

            private IDictionary<string, string> GetTldDictionary()
            {
                string[] tlds = GetTlds();
                IDictionary<string, string> dic = new Dictionary<string, string>(tlds.Length);
                foreach (string tld in tlds)
                {
                    if (!dic.ContainsKey(tld))
                        dic.Add(tld, tld);
                }
                return dic;
            }

        }

    }
}
