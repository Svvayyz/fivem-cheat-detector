using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace fivemhackdetector
{
    internal class CModulesWhitelist
    {
        private WebClient pClient = new WebClient();

        public bool bSuccess = false;
        public List<string> mWhitelist;

        public CModulesWhitelist() {
            string body = "";
            try
            {
                body = pClient.DownloadString("https://api.amethyst.rip/stuff/fivem/whitelist.json");
            }
            catch (Exception)
            {
                return;
            }

            mWhitelist = JsonConvert.DeserializeObject<List<string>>(
                body
            );

            bSuccess = true;
        }
    }
}
