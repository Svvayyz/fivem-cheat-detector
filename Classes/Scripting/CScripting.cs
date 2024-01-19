using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fivemhackdetector.Classes.Mods;
using NLua;

namespace fivemhackdetector.Classes.Scripting
{
    internal class CScripting
    {
        private string szScriptName = "";

        class CScriptingConsole
        {
            private string szScriptName = "";

            public CScriptingConsole(string scriptName)
            {
                szScriptName = scriptName;
            }

            public void Log(string str)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(DateTime.Now);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] [");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(szScriptName);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] " + str + "\n");
            }

            public void Log(bool b)
            {
                Log(b.ToString());
            }
            public void Log(int i)
            {
                Log(i.ToString());
            }

            public void SetTitle(string str)
            {
                Console.Title = str;
            }
        }
        class CScriptingGTAProcess
        {
            public bool Exists()
            {
                return Program.cProcesses.Get().Count > 0;
            }

            public ProcessModuleCollection GetModules()
            {
                return Program.cProcesses.Get().First().Modules; 
            }

            public List<CMemoryString> GetStrings()
            {
                return new CMemory(Program.cProcesses.Get().First()).GetStrings();
            }
        }
        class CScriptingString
        {
            public string get(byte[] bytes)
            {
                return Encoding.ASCII.GetString(bytes);
            }
        }

        public class CScriptingCallbacks
        {
            public Dictionary<string, List<LuaFunction>> mCallbacks = new Dictionary<string, List<LuaFunction>>();

            public bool KeyExists(string key)
            {
                List<LuaFunction> list = new List<LuaFunction>();
                return mCallbacks.TryGetValue(key, out list);
            }

            public void Add(string str, LuaFunction function)
            {
                List<LuaFunction> list = new List<LuaFunction>();
                bool success = mCallbacks.TryGetValue(str, out list);

                if (!success)
                    list = new List<LuaFunction>();

                list.Add(function);

                mCallbacks[str] = list;
            }
        }

        public Lua pState = new Lua();
        public CScripting() {
            Initialize();
        }

        public void Initialize()
        {
            if (!Directory.Exists("scripts"))
                Directory.CreateDirectory("scripts");
        }

        public void Load(string path)
        {
            pState = new Lua();

            szScriptName = Path.GetFileName(path);

            pState["Console"] = new CScriptingConsole(szScriptName);
            pState["FiveM"] = new CScriptingGTAProcess();
            pState["Prefetch"] = Program.cPrefetch;
            pState["Mods"] = Program.cMods;
            pState["Callbacks"] = Program.cCallbacks;
            pState["String"] = new CScriptingString();

            try
            {
                pState.DoFile(path);
            }
            catch (Exception ex)
            {
                Program.cConsole.Log(CConsole.LogType.ERROR, $"{ex.Message}");
            }
        }
    }
}
