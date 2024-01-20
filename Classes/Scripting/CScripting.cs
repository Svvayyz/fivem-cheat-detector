using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using NLua;

namespace fivemhackdetector.Classes.Scripting
{
    internal class CScripting
    {
        private string szScriptName = "";
        private string szLastError = "";

        class CScriptingConsole
        {
            private string szScriptName = "";
            private CConsole cConsole;

            public CScriptingConsole(string scriptName, CConsole console)
            {
                szScriptName = scriptName;
                cConsole = console;
            }

            public void Log(string str)
            {
                cConsole.Log($"%gray%[  %green%OK%gray%  ] [%blue%{szScriptName}%gray%] {str}", true);
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
            public string Get(byte[] bytes)
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

        private CConsole cConsole;

        public CScripting(CConsole console) {
            cConsole = console;

            Initialize();
        }

        public void Initialize()
        {
            if (!Directory.Exists("scripts"))
                Directory.CreateDirectory("scripts");
        }
        public string GetLastError()
        {
            return szLastError;
        }

        public bool Load(string path)
        {
            pState = new Lua();

            szScriptName = Path.GetFileName(path);

            pState["Console"] = new CScriptingConsole(szScriptName, cConsole);
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
                szLastError = ex.Message;

                return false;
            }

            return true;
        }
    }
}
