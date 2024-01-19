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
                         $"fount a suspicious (unknown) module:\n" +
                         $" - File:\n" +
                         $" \t - Name: {module.ModuleName}\n" +
                         $" \t - Path: {module.FileName}\n\n" +
                         $" \t - Created At: {File.GetCreationTime(module.FileName)}\n" +
                         $" \t - Modified At: {File.GetLastWriteTime(module.FileName)}\n" +
                         $" \t - Accessed At: {File.GetLastAccessTime(module.FileName)}\n\n" +

                         $" - Memory:\n" +
                         $" \t - Start Address: 0x{module.BaseAddress}\n" +
                         $" \t - End Address: 0x{module.BaseAddress + module.ModuleMemorySize}\n\n" +
                         $" \t - Size: {module.ModuleMemorySize}"
                     );
                }
            }

            return result;
        }
    }
}
