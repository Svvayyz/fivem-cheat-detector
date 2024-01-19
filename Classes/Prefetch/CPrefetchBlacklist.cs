using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace fivemhackdetector.Classes.Prefetch
{
    [Serializable]
    public class CPrefetchBlacklistedFile
    {
        [JsonProperty("name")]
        public string szName { get; set; }
        [JsonProperty("Description")]
        public string szDescription { get; set; }
    }

    internal class CPrefetchBlacklist
    {
        private WebClient pClient = new WebClient();

        public bool bSuccess = false;
        public Dictionary<string, CPrefetchBlacklistedFile> mBlacklist;

        public CPrefetchBlacklist()
        {
            string body = "";
            try
            {
                body = pClient.DownloadString("https://api.amethyst.rip/stuff/fivem/blacklist.json");
            }
            catch (Exception)
            {
                return;
            }

            mBlacklist = JsonConvert.DeserializeObject<Dictionary<string, CPrefetchBlacklistedFile>>(
                body
            );

            bSuccess = true;
        }
    }
}
