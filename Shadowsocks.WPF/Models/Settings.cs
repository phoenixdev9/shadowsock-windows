using Shadowsocks.Interop.Settings;
using Shadowsocks.Models;
using Shadowsocks.Net.Settings;
using Shadowsocks.PAC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.WPF.Models
{
    public class Settings
    {
        public AppSettings App { get; set; }

        public InteropSettings Interop { get; set; }
        
        public NetSettings Net { get; set; }

        public PACSettings PAC { get; set; }
        
        public List<Group> Groups { get; set; }
        
        public Settings()
        {
            App = new AppSettings();
            Interop = new InteropSettings();
            Net = new NetSettings();
            PAC = new PACSettings();
            Groups = new List<Group>();
        }
    }
}
