using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NLua;

using fivemhackdetector.Classes.Utilities;

namespace fivemhackdetector
{
    internal class CModules
    {
        private Process pProcess;
        private List<string> mTrustedModules;

        private CGTA cGTA = new CGTA();

        public CModules(Process proc, List<string> trustedModules) {
            pProcess = proc;
            mTrustedModules = trustedModules;
        }

        public List<string> Check()
        {
            List<string> result = new List<string>();

            foreach (ProcessModule module in pProcess.Modules)
            {
                bool bFound = false;
                foreach (string trustedModule in mTrustedModules)
                {
                    if (cGTA.ExpandPath(trustedModule) == module.FileName)
                        bFound = true;    
                }

                if (Program.cCallbacks.KeyExists("OnModuleProcessing"))
                {
                    foreach (LuaFunction func in Program.cCallbacks.mCallbacks["OnModuleProcessing"])
                    {
                        bFound = bool.Parse(func.Call(module.FileName, bFound)[0].ToString());
                    }
                }

                if (!bFound)
                {
                     result.Add(
                         $"found a %yellow%suspicious%gray% (unknown) %blue%module%gray%:\n" +
                         $" - %yellow%File%gray%:\n" +
                         $" \t - Name: %yellow%{module.ModuleName}%gray%\n" +
                         $" \t - Path: %yellow%{module.FileName}%gray%\n\n" +

                         $" \t - Created At: %yellow%{File.GetCreationTime(module.FileName)}%gray%\n" +
                         $" \t - Modified At: %yellow%{File.GetLastWriteTime(module.FileName)}%gray%\n" +
                         $" \t - Accessed At: %yellow%{File.GetLastAccessTime(module.FileName)}%gray%\n\n" +

                         $" - %blue%Memory%gray%:\n" +
                         $" \t - Start Address: %blue%0x{module.BaseAddress}%gray%\n" +
                         $" \t - End Address: %blue%0x{module.BaseAddress + module.ModuleMemorySize}%gray%\n\n" +
                         $" \t - Size: %blue%{module.ModuleMemorySize}%gray%"
                     );
                }
            }

            return result;
        }
    }
}
