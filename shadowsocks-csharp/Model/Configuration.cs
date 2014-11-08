﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shadowsocks.Model
{
    [Serializable]
    public class Configuration
    {
        public Configuration()
        {
        }

        public List<Server> configs;
        public int index;
        public bool enabled;
        public bool isDefault;

        private static string CONFIG_FILE = "gui-config.json";

        public Server GetCurrentServer()
        {
            if (index >= 0 && index < configs.Count)
            {
                return configs[index];
            }
            else
            {
                return GetDefaultServer();
            }
        }

        public static void CheckServer(Server server)
        {
            checkPort(server.local_port);
            checkPort(server.server_port);
            checkPassword(server.password);
            checkServer(server.server);
            checkRemark(server.remarks);
        }

        public static Configuration Load()
        {
            try
            {
                string configContent = File.ReadAllText(CONFIG_FILE);
                Configuration config = SimpleJson.SimpleJson.DeserializeObject<Configuration>(configContent, new JsonSerializerStrategy());
                config.isDefault = false;
                return config;
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    Console.WriteLine(e);
                }
                return new Configuration
                {
                    index = 0,
                    isDefault = true,
                    configs = new List<Server>()
                    {
                        GetDefaultServer()
                    }
                };
            }
        }

        public static void Save(Configuration config)
        {
            if (config.index >= config.configs.Count)
            {
                config.index = config.configs.Count - 1;
            }
            if (config.index < 0)
            {
                config.index = 0;
            }
            config.isDefault = false;
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(CONFIG_FILE, FileMode.Create)))
                {
                    string jsonString = SimpleJson.SimpleJson.SerializeObject(config);
                    sw.Write(jsonString);
                    sw.Flush();
                }
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public static Server GetDefaultServer()
        {
            return new Server()
            {
                server = "",
                server_port = 8388,
                local_port = 1080,
                method = "aes-256-cfb",
                password = "",
                remarks = ""
            };
        }

        private static void assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("assertion failure");
            }
        }

        private static void checkPort(int port)
        {
            if (port <= 0 || port > 65535)
            {
                throw new ArgumentException("port out of range");
            }
        }

        private static void checkPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password can not be blank");
            }
        }

        private static void checkServer(string server)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException("server IP can not be blank");
            }
        }

        private static void checkRemark(string remark)
        {
            //remark is optional
        }

        private class JsonSerializerStrategy : SimpleJson.PocoJsonSerializerStrategy
        {
            // convert string to int
            public override object DeserializeObject(object value, Type type)
            {
                if (type == typeof(Int32) && value.GetType() == typeof(string))
                {
                    return Int32.Parse(value.ToString());
                }
                return base.DeserializeObject(value, type);
            }
        }
    }
}
