﻿using Shadowsocks.Model;
using Shadowsocks.Properties;
using Shadowsocks.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Shadowsocks.Controller
{
    class PACServer : Listener.Service
    {
        public static string PAC_FILE = "pac.txt";

        FileSystemWatcher watcher;
        private Configuration _config;

        public event EventHandler PACFileChanged;

        public PACServer()
        {
            this.WatchPacFile();
        }

        public void UpdateConfiguration(Configuration config)
        {
            this._config = config;
        }

        public bool Handle(byte[] firstPacket, int length, Socket socket)
        {
            try
            {
                string request = Encoding.UTF8.GetString(firstPacket, 0, length);
                string[] lines = request.Split('\r', '\n');
                bool hostMatch = false, pathMatch = false;
                foreach (string line in lines)
                {
                    string[] kv = line.Split(new char[]{':'}, 2);
                    if (kv.Length == 2)
                    {
                        if (kv[0] == "Host")
                        {
                            if (kv[1].Trim() == ((IPEndPoint)socket.LocalEndPoint).ToString())
                            {
                                hostMatch = true;
                            }
                        }
                    }
                    else if (kv.Length == 1)
                    {
                        if (line.IndexOf("pac") >= 0)
                        {
                            pathMatch = true;
                        }
                    }
                }
                if (hostMatch && pathMatch)
                {
                    SendResponse(firstPacket, length, socket);
                    return true;
                }
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }


        public string TouchPACFile()
        {
            if (File.Exists(PAC_FILE))
            {
                return PAC_FILE;
            }
            else
            {
                FileManager.UncompressFile(PAC_FILE, Resources.proxy_pac_txt);
                return PAC_FILE;
            }
        }

        private string GetPACContent()
        {
            if (File.Exists(PAC_FILE))
            {
                return File.ReadAllText(PAC_FILE, Encoding.UTF8);
            }
            else
            {
                return Utils.UnGzip(Resources.proxy_pac_txt);
            }
        }

        public void SendResponse(byte[] firstPacket, int length, Socket socket)
        {
            try
            {
                string pac = GetPACContent();

                IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;

                string proxy = GetPACAddress(firstPacket, length, localEndPoint);

                pac = pac.Replace("__PROXY__", proxy);

                string text = String.Format(@"HTTP/1.1 200 OK
Server: Shadowsocks
Content-Type: application/x-ns-proxy-autoconfig
Content-Length: {0}
Connection: Close

", System.Text.Encoding.UTF8.GetBytes(pac).Length) + pac;
                byte[] response = System.Text.Encoding.UTF8.GetBytes(text);
                socket.BeginSend(response, 0, response.Length, 0, new AsyncCallback(SendCallback), socket);
                Util.Utils.ReleaseMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                socket.Close();
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket conn = (Socket)ar.AsyncState;
            try
            {
                conn.Shutdown(SocketShutdown.Send);
            }
            catch
            { }
        }

        private void WatchPacFile()
        {
            if (watcher != null)
            {
                watcher.Dispose();
            }
            watcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = PAC_FILE;
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (PACFileChanged != null)
            {
                PACFileChanged(this, new EventArgs());
            }
        }

        private string GetPACAddress(byte[] requestBuf, int length, IPEndPoint localEndPoint)
        {
            return "PROXY " + localEndPoint.Address + ":" + this._config.localPort + ";";
        }
    }
}
