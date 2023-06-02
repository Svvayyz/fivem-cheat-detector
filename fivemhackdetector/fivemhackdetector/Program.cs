using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace fivemhackdetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Scanner | Svvayyz#7153";

            string trustedModules = new WebClient().DownloadString("https://adorablehvh.pl/trusted.txt");

            Log(LogType.SUCCESS, "begun scanning!");

            for (int i = 0; i < Process.GetProcesses().Length; i++)
            {
                Process process = Process.GetProcesses()[i];

                if (process.ProcessName.Contains("GTAProcess"))
                {
                    Log(LogType.SUCCESS, $"fount fivem process with id of {process.Id}");

                    for (int i2 = 0; i2 < process.Modules.Count; i2++)
                    {
                        ProcessModule module = process.Modules[i2];

                        if (!trustedModules.Contains(module.ModuleName) && !module.ModuleName.EndsWith(".exe")) { Log(LogType.WARNING, $"fount a suspicious (unknown) module {module.ModuleName}"); }
                    }
                }
            }

            Log(LogType.SUCCESS, "finished scanning!");
            Console.ReadLine();
        }

        enum LogType {
            SUCCESS,
            WARNING
        }

        private static void Log(LogType type, string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");

            if (type == LogType.SUCCESS)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("success");
            }
            else if (type == LogType.WARNING)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("warning");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] " + message + "\n");
        }
    }
}
