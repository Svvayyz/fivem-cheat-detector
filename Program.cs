using System;
using System.Diagnostics;
using System.Net;

namespace fivemhackdetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Scanner | Svvayyz#7153";

            string trustedModules = new WebClient().DownloadString("https://adorablehvh.pl/trusted.txt");
            bool fivemDetected = false;

            Utilities.Log(Utilities.LogType.SUCCESS, "begun scanning!");

            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.Contains("GTAProcess"))
                {
                    fivemDetected = true;

                    Utilities.Log(Utilities.LogType.SUCCESS, $"fount fivem process with id of {process.Id}");

                    foreach (ProcessModule module in process.Modules)
                    {
                        if (!trustedModules.Contains(module.ModuleName) && !module.ModuleName.EndsWith(".exe"))
                            Utilities.Log(Utilities.LogType.WARNING, $"fount a suspicious (unknown) module {module.ModuleName} | MEM: 0x{module.BaseAddress}-0x{module.BaseAddress + module.ModuleMemorySize} (size: {module.ModuleMemorySize})");
                    }
                }
            }

            if (!fivemDetected) 
                Utilities.Log(Utilities.LogType.ERROR, "fivem not found");

            Utilities.Log(Utilities.LogType.SUCCESS, "finished scanning!");
            Console.ReadLine();
        }
    }
}
