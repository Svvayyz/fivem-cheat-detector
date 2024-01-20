using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using NLua;

using fivemhackdetector.Classes;
using fivemhackdetector.Classes.Mods;
using fivemhackdetector.Classes.Prefetch;
using fivemhackdetector.Classes.Scripting;

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
            cConsole.Refresh();
            cConsole.Log("%gray%[  %green%OK%gray%  ] Init%blue%ialized%gray%!", true);

            #region scripting
            CScripting cScripting = new CScripting(cConsole);

            foreach (string path in Directory.GetFiles("scripts")) {
                cConsole.Log($"         Loading %blue%{Path.GetFileName(path)}%gray%!", false);

                bool success = cScripting.Load(path);

                cConsole.Refresh();

                if (success)
                    cConsole.Log($"%gray%[  %green%OK%gray%  ] Loaded %blue%{Path.GetFileName(path)}%gray%!", true);
                else
                    cConsole.Log($"%gray%[%red%FAILED%gray%] Failed to load %red%{Path.GetFileName(path)}%gray% (%red%{cScripting.GetLastError()}%gray%)!", true);
            }
            #endregion scripting

            cConsole.Log("%gray%[  %green%OK%gray%  ] The %blue%process%gray% of %blue%scanning%gray% has %blue%began%gray%!", true);

            #region process
            foreach (Process process in cProcesses.Get())
            {
                cConsole.Log($"%gray%[  %green%OK%gray%  ] Found %blue%FiveM process%gray% with %blue%id%gray% of %blue%{process.Id}!", true);

                #region modules
                if (!cWhitelist.bSuccess)
                    return;

                cConsole.Log("         Scanning ", false);

                CModules cModules = new CModules(process, cWhitelist.mWhitelist);
                foreach (string mod in cModules.Check())
                {
                    cConsole.Log(mod, true);
                }

                cConsole.Refresh();
                cConsole.Log("%gray%[  %green%OK%gray%  ] Finished %blue%scanning%gray% modules!", true);
                #endregion modules
                #region memory
                if (!cBlacklist.bSuccess)
                    return;

                CMemory cMemory = new CMemory(process);
                List<CMemoryString> strings = cMemory.GetStrings();

                cConsole.Log($"         Found %blue%{strings.Count} string(s)%gray%, starting %blue%processing%gray%!", false);

                DateTime lastUpdate = DateTime.Now;

                for (int i = 0; i < strings.Count; i++) {
                    CMemoryString str = strings[i];

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
                        cConsole.Log(st, true);

                    if ((DateTime.Now - lastUpdate).TotalMilliseconds > 1000)
                    {
                        cConsole.Refresh();

                        float progress = (float.Parse(i.ToString()) / float.Parse(strings.Count.ToString())) * 100;

                        cConsole.ProgressBar($"Scanning %blue%strings%gray% {i} / %blue%{strings.Count}%gray%", progress, "StringScanProgressBar");

                        lastUpdate = DateTime.Now;
                    }
                }

                cConsole.Refresh();
                cConsole.Log("%gray%[  %green%OK%gray%  ] Finished scanning %blue%strings%gray%!", true);
                #endregion memory
            }
            #endregion process

            if (!cProcesses.bFound)
                cConsole.Log("%gray%[%red%FAILED%gray%] %red%FiveM%gray% not found!", true);

            #region files
            #region mods 
            List<CFiveMMod> mods = cMods.Get();

            cConsole.Log($"%gray%[  %green%OK%gray%  ] Found %blue%{mods.Count} mod(s)%gray%, starting %blue%processing", false);

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
                    cConsole.Log(
                        "found an unknown mod:\n" +
                        $" - Name: {mod.szName}\n" +
                        $" - Path: {mod.szPath}",
                        true
                    );
            }

            cConsole.Refresh();
            cConsole.Log("%gray%[  %green%OK%gray%  ] Finished processing %blue%mods%gray%!", true);
            #endregion mods
            #region prefetch
            List<CPrefetchFile> entries = cPrefetch.GetFiles();

            cConsole.Log($"%gray%[  %green%OK%gray%  ] Found %blue%{entries.Count} prefetch entry(ies)%gray%, starting %blue%processing!", false);

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

                cConsole.Log(
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
                    $" \t - Accessed At: {File.GetLastAccessTime(file.szPath)}",
                    true
                );
            }

            cConsole.Refresh();
            cConsole.Log("%gray%[  %green%OK%gray%  ] Finished processing %blue%prefetch%gray%!", true);
            #endregion prefetch
            #endregion files

            cConsole.Log("%gray%[  %green%OK%gray%  ] Finished %blue%scanning%gray%!", true);

            cConsole.Log("         Saving %blue%log", false);

            string szLogPath = cConsole.SaveLog();

            cConsole.Refresh();
            cConsole.Log($"%gray%[  %green%OK%gray%  ] Log has been %blue%saved%gray% to %blue%{szLogPath}%gray%!", true);

            Console.ReadLine();
        }
    }
}
