using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fivemhackdetector
{
    internal class CConsole
    {
        public CConsole(string title)
        {
            Log(LogType.SUCCESS, "initializing");

            Console.Title = title;
        }

        public enum LogType
        {
            SUCCESS,
            WARNING,
            ERROR
        }

        private static Dictionary<LogType, ConsoleColor> colors = new Dictionary<LogType, ConsoleColor>()
        {
            { LogType.SUCCESS, ConsoleColor.Blue },
            { LogType.WARNING, ConsoleColor.Yellow },
            { LogType.ERROR, ConsoleColor.Red }
        };

        private static Dictionary<LogType, string> prefixes = new Dictionary<LogType, string>()
        {
            { LogType.SUCCESS, "success" },
            { LogType.WARNING, "warning" },
            { LogType.ERROR, "error" }
        };

        public void Log(LogType type, string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(DateTime.Now);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] [");

            Console.ForegroundColor = colors[type];
            Console.Write(prefixes[type]);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] " + message + "\n");
        }
    }
}
