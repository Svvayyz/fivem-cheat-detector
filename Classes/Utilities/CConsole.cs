using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace fivemhackdetector
{
    internal class CConsole
    {
        public CConsole(string title)
        {
            Log("         Init%blue%ializing", false);

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

        private static Dictionary<string, ConsoleColor> colorsByNames = new Dictionary<string, ConsoleColor>()
        {
            { "black", ConsoleColor.Black },
            { "darkblue", ConsoleColor.DarkBlue },
            { "darkgreen", ConsoleColor.DarkGreen },
            { "darkcyan", ConsoleColor.DarkCyan },
            { "darkred", ConsoleColor.DarkRed },
            { "darkmagenta", ConsoleColor.DarkMagenta },
            { "darkyellow", ConsoleColor.DarkYellow },
            { "gray", ConsoleColor.Gray },
            { "darkgray", ConsoleColor.DarkGray },
            { "blue", ConsoleColor.Blue },
            { "green", ConsoleColor.Green },
            { "cyan", ConsoleColor.Cyan },
            { "red", ConsoleColor.Red },
            { "magenta", ConsoleColor.Magenta },
            { "yellow", ConsoleColor.Yellow },
            { "white", ConsoleColor.White }
        };

        private string szMessages = ""; 

        private void Print(string message, bool add)
        {
            if (add)
                szMessages += message + "\n";

            string[] splitted = message.Split('%');

            if (splitted.Length > 2)
            {
                for (int i = 0; i < splitted.Length; i++)
                {
                    ConsoleColor color = ConsoleColor.White;
                    bool success = colorsByNames.TryGetValue(splitted[i], out color);

                    if (success)
                    {
                        Console.ForegroundColor = color;
                    }
                    else
                    {
                        Console.Write(splitted[i]);
                    }
                }
            }
            else
            {
                Console.Write(message);
            }
        }
        public void Refresh()
        {
            Console.Clear();

            Print(szMessages, false);
        }
        public void Log(string message, bool add)
        {
            Print($"%gray%[%blue%{DateTime.Now.ToString("HH:mm:ss.fff")}%gray%] " + message, add);

            Console.Write("\n");
        }

        private static string animation = @"|/-\";
        private static Dictionary<string, int> animationKeyFrame = new Dictionary<string, int>();
        public void ProgressBar(string prefix, float progress, string name)
        {
            int keyframe = 0;
            bool success = animationKeyFrame.TryGetValue(name, out keyframe);

            if (!success)
                animationKeyFrame.Add(name, 0);

            string progressString = progress.ToString().Replace(",", ".");

            string progressBarString = "";
            for (int k = 0; k < 6; k++)
            {
                if (k > ((progress / 100) * 6))
                {
                    progressBarString += "%gray%-";
                }
                else
                {
                    progressBarString += "%blue%#";
                }
            }

            Log($"[{progressBarString}] {prefix} (%blue%{progressString}%gray% {animation[keyframe % animation.Length]})", false);

            animationKeyFrame[name] += 1;
        }

        public string SaveLog()
        {
            string log = szMessages;

            foreach (KeyValuePair<string, ConsoleColor> value in colorsByNames)
            {
                log = log.Replace("%" + value.Key + "%", "");
            }

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string path = $"logs\\{DateTime.Now.ToString("dd.MM.yyyy HH.MM.ss")}.log";
            FileStream stream = File.Create(path);

            byte[] bytes = Encoding.Default.GetBytes(log);
            stream.Write(bytes, 0, bytes.Length);

            stream.Close();

            return path;
        }
    }
}
