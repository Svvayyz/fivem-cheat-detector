using System;
using System.Collections.Generic;
using System.IO;

namespace fivemhackdetector.Classes.Mods
{
    public class CFiveMMod
    {
        public string szPath = "";
        public string szName = "";

        public CFiveMMod(string path)
        {
            szPath = path;
            szName = Path.GetFileName(szPath);
        }
    }
    internal class CFiveMMods
    {
        private string szModsPath = Environment.ExpandEnvironmentVariables("%localappdata%") + "\\FiveM\\FiveM.app\\mods";

        public List<CFiveMMod> mMods = new List<CFiveMMod>();
        public bool bSuccess = false;

        public CFiveMMods()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!Directory.Exists(szModsPath))
                return;

            foreach (string path in Directory.GetFiles(szModsPath))
            {
                if (path.EndsWith("sculpture_revival.rpf"))
                    continue;

                CFiveMMod mod = new CFiveMMod(path);

                mMods.Add(mod);
            }

            bSuccess = true;
        }
        public List<CFiveMMod> Get()
        {
            return mMods;
        }
    }
}
