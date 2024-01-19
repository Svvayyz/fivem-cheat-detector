using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using fivemhackdetector.Classes;
using fivemhackdetector.Classes.Mods;
using fivemhackdetector.Classes.Prefetch;
using fivemhackdetector.Classes.Scripting;
using fivemhackdetector.Classes.Utilities;
using NLua;
using static fivemhackdetector.Classes.Scripting.CScripting;

namespace fivemhackdetector
{
    internal class Program
    {
        public static CConsole cConsole = new CConsole("Scanner \\ Svvayyz");
        public static CGTAProcess cProcesses = new CGTAProcess();
        public static CModulesWhitelist cWhitelist = new CModulesWhitelist();
        public static CPrefetchBlacklist cBlacklist = new CPrefetchBlacklist();
        public static CPrefetch cPrefetch = new CPrefetch();
        public static CFiveMMods cMods = new CFiveMMods();
        public static CScriptingCallbacks cCallbacks = new CScriptingCallbacks();

        static void Main(string[] args)
        {
            cConsole.Log(CConsole.LogType.SUCCESS, "initialized");

            #region scripting
            CScripting cScripting = new CScripting();

            foreach (string path in Directory.GetFiles("scripts")) {
                cConsole.Log(CConsole.LogType.SUCCESS, $"loading {Path.GetFileName(path)}!");

                cScripting.Load(path);
            }
            #endregion scripting

            cConsole.Log(CConsole.LogType.SUCCESS, "the process of scanning has began!");

            #region process
            foreach (Process process in cProcesses.Get())
            {
                cConsole.Log(CConsole.LogType.SUCCESS, $"fount fivem process with id of {process.Id}");

                #region modules
                if (!cWhitelist.bSuccess)
                    return;

                CModules cModules = new CModules(process, cWhitelist.mWhitelist);
                foreach (string mod in cModules.Check())
                {
                    cConsole.Log(CConsole.LogType.WARNING, mod);
                }

                cConsole.Log(CConsole.LogType.SUCCESS, "finished scanning modules, beggining scanning memory");
                #endregion modules
                #region memory
                if (!cBlacklist.bSuccess)
                    return;

                CMemory cMemory = new CMemory(process);
                List<CMemoryString> strings = cMemory.GetStrings();

                cConsole.Log(CConsole.LogType.SUCCESS, $"found {strings.Count} strings, starting processing");

                foreach (CMemoryString str in strings) {
                    string st = Encoding.ASCII.GetString(str.szStr);

                    bool bFound = false;

                    if (st.Contains("citizen"))
                        bFound = true;

                    if (cCallbacks.KeyExists("OnStringProcessing"))
                    {
                        foreach (LuaFunction func in cCallbacks.mCallbacks["OnStringProcessing"])
                        {
                            bFound = bool.Parse(func.Call(st, str.iLenght, str.uAddress, bFound)[0].ToString());
                        }
                    }

                    if (bFound)
                        cConsole.Log(CConsole.LogType.WARNING, st);
                }
                #endregion memory
            }
            #endregion process

            if (!cProcesses.bFound)
                cConsole.Log(CConsole.LogType.ERROR, "fivem not found");

            #region files
            #region mods 
            List<CFiveMMod> mods = cMods.Get();

            cConsole.Log(CConsole.LogType.SUCCESS, $"found {mods.Count} mods, starting processing");

            foreach (CFiveMMod mod in mods)
            {
                bool bFound = true;

                if (cCallbacks.KeyExists("OnModProcessing"))
                {
                    foreach (LuaFunction func in cCallbacks.mCallbacks["OnModProcessing"])
                    {
                        bFound = bool.Parse(func.Call(mod, bFound)[0].ToString());
                    }
                }

                if (bFound)
                    cConsole.Log(CConsole.LogType.WARNING,
                        "found an unknown mod:\n" +
                        $" - Name: {mod.szName}\n" +
                        $" - Path: {mod.szPath}"
                    );
            }

            cConsole.Log(CConsole.LogType.SUCCESS, "finished processing mods, beggining scanning prefetch");
            #endregion mods
            #region prefetch
            List<CPrefetchFile> entries = cPrefetch.GetFiles();

            cConsole.Log(CConsole.LogType.SUCCESS, $"found {entries.Count} prefetch entries, starting processing");

            foreach (CPrefetchFile file in entries)
            {
                CPrefetchBlacklistedFile info = null;

                bool success = cBlacklist.mBlacklist.TryGetValue(file.szCheatName, out info);
                if (!success)
                    success = cBlacklist.mBlacklist.TryGetValue(file.szHash, out info);

                if (cCallbacks.KeyExists("OnPrefetchProcessing"))
                {
                    foreach (LuaFunction func in cCallbacks.mCallbacks["OnPrefetchProcessing"])
                    {
                        success = bool.Parse(func.Call(file, success)[0].ToString());
                    }
                }

                if (!success)
                    continue;

                cConsole.Log(CConsole.LogType.WARNING,
                    $"found a cheat's loader:\n" +
                    $" - Info:\n" +
                    $" \t - Name: {info.szName}\n" +
                    $" \t - Description: {info.szDescription}\n\n" +

                    $" - File: \n" +
                    $" \t - Name: {file.szExecutableName}\n" +
                    $" \t - Path: {file.szPath}\n" +
                    $" \t - MD5 Hash: {file.szHash}\n\n" +

                    $" \t - Created At: {File.GetCreationTime(file.szPath)}\n" +
                    $" \t - Modified At: {File.GetLastWriteTime(file.szPath)}\n" +
                    $" \t - Accessed At: {File.GetLastAccessTime(file.szPath)}"
                );
            }

            cConsole.Log(CConsole.LogType.SUCCESS, "finished processing prefetch");
            #endregion prefetch
            #endregion files

            cConsole.Log(CConsole.LogType.SUCCESS, "finished scanning!");

            Console.ReadLine();
        }
    }
}
