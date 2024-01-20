using System;
using System.IO;

namespace fivemhackdetector.Classes.Utilities
{
    internal class CGTA
    {
        public string szPath = "";

        public CGTA()
        {
            Localize();
        }

        public string ExpandPath(string str)
        {
            return Environment.ExpandEnvironmentVariables(
                str.Replace("%gtadir%", szPath)
            );
        }

        private void Localize()
        {
            string path = Environment.ExpandEnvironmentVariables("%localappdata%") + "\\FiveM\\FiveM.app\\CitizenFX.ini";
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                if (!line.StartsWith("IVPath"))
                    continue;

                string[] args = line.Split('=');

                szPath = args[1];
            }
        }
    }
}
