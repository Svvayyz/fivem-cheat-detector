﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace fivemhackdetector
{
    internal class Program
    {
        private static string[] _trustedModules =
        {
            "ntdll.dll",
            "KERNEL32.DLL",
            "KERNELBASE.dll",
            "USER32.dll",
            "win32u.dll",
            "GDI32.dll",
            "gdi32full.dll",
            "msvcp_win.dll",
            "ucrtbase.dll",
            "ADVAPI32.dll",
            "msvcrt.dll",
            "sechost.dll",
            "RPCRT4.dll",
            "SHELL32.dll",
            "MSVCP140.dll",
            "ole32.dll",
            "VCRUNTIME140.dll",
            "combase.dll",
            "VCRUNTIME140_1.dll",
            "IMM32.DLL",
            "steam_api64.dll",
            "icuuc.dll",
            "icui18n.dll",
            "CoreRT.dll",
            "SHLWAPI.dll",
            "VERSION.dll",
            "windows.storage.dll",
            "Wldp.dll",
            "SHCORE.dll",
            "profapi.dll",
            "adhesive.dll",
            "CRYPT32.dll",
            "WS2_32.dll",
            "WINTRUST.dll",
            "bcrypt.dll",
            "OLEAUT32.dll",
            "WINMM.dll",
            "v8-9.3.345.16.dll",
            "rage-nutsnbolts-five.dll",
            "net.dll",
            "citizen-resources-client.dll",
            "ros-patches-five.dll",
            "rage-device-five.dll",
            "net-tcp-server.dll",
            "gta-core-five.dll",
            "net-base.dll",
            "libuv.dll",
            "citizen-resources-core.dll",
            "PSAPI.DLL",
            "vfs-core.dll",
            "citizen-scripting-core.dll",
            "scripting-gta.dll",
            "rage-scripting-five.dll",
            "conhost-v2.dll",
            "dbghelp.dll",
            "CONCRT140.dll",
            "http-client.dll",
            "steam.dll",
            "cfx_curl_x86_64.dll",
            "botan.dll",
            "net-http-server.dll",
            "mojo.dll",
            "WININET.dll",
            "SETUPAPI.dll",
            "cfgmgr32.dll",
            "rage-allocator-five.dll",
            "rage-input-five.dll",
            "DNSAPI.dll",
            "rage-graphics-five.dll",
            "USERENV.dll",
            "IPHLPAPI.DLL",
            "nui-core.dll",
            "D3DCOMPILER_47.dll",
            "pdh.dll",
            "dwmapi.dll",
            "PROPSYS.dll",
            "d3d11.dll",
            "imgui.dll",
            "dxgi.dll",
            "WindowsCodecs.dll",
            "CRYPTBASE.DLL",
            "NSI.dll",
            "MSASN1.dll",
            "bcryptPrimitives.dll",
            "imagehlp.dll",
            "CitiLaunch_TLSDummy.dll",
            "dinput8.dll",
            "dsound.dll",
            "powrprof.dll",
            "winmmbase.dll",
            "UMPDC.dll",
            "xinput9_1_0.dll",
            "inputhost.dll",
            "CoreMessaging.dll",
            "CoreUIComponents.dll",
            "kernel.appcore.dll",
            "ntmarta.dll",
            "wintypes.dll",
            "xinput1_1.dll",
            "xinput1_2.dll",
            "xinput1_3.dll",
            "xinput1_4.dll",
            "DEVOBJ.dll",
            "d3d9.dll",
            "d3d10.dll",
            "d3d10core.dll",
            "d3d10_1.dll",
            "d3d10_1core.dll",
            "opengl32.dll",
            "GLU32.dll",
            "CitizenGame.dll",
            "COMCTL32.dll",
            "scripthookv.dll",
            "gta-mission-cleanup-five.dll",
            "cryptnet.dll",
            "drvstore.dll",
            "nvapi64.dll",
            "citizen-scripting-v8client.dll",
            "gta-game-five.dll",
            "rage-formats-x.dll",
            "gta-streaming-five.dll",
            "citizen-level-loader-five.dll",
            "profiles.dll",
            "scrbind-base.dll",
            "voip-mumble.dll",
            "avutil-56.dll",
            "swresample-3.dll",
            "AVRT.dll",
            "gta-net-five.dll",
            "citizen-resources-gta.dll",
            "font-renderer.dll",
            "DWrite.dll",
            "nui-resources.dll",
            "devcon.dll",
            "citizen-legacy-net-resources.dll",
            "debug-net.dll",
            "citizen-game-ipc.dll",
            "nng.dll",
            "citizen-game-main.dll",
            "asi-five.dll",
            "lovely-script.dll",
            "extra-natives-five.dll",
            "vfs-impl-rage.dll",
            "glue.dll",
            "secur32.dll",
            "SSPICLI.DLL",
            "loading-screens-five.dll",
            "citizen-scripting-lua.dll",
            "handling-loader-five.dll",
            "nui-gsclient.dll",
            "citizen-scripting-mono.dll",
            "mono-2.0-sgen.dll",
            "MSWSOCK.dll",
            "citizen-devtools.dll",
            "devtools-five.dll",
            "nui-profiles.dll",
            "tool-formats.dll",
            "citizen-resources-metadata-lua.dll",
            "citizen-mod-loader-five.dll",
            "citizen-scripting-v8node.dll",
            "node.dll",
            "fxdk-main.dll",
            "citizen-playernames-five.dll",
            "citizen-scripting-lua54.dll",
            "discord.dll",
            "citizen-scripting-mono-v2.dll",
            "tool-vehrec.dll",
            "bluetoothapis.dll",
            "chrome_elf.dll",
            "libEGL.dll",
            "libGLESv2.dll",
            "libcef.dll",
            "COMDLG32.dll",
            "UIAutomationCore.DLL",
            "OLEACC.dll",
            "NETAPI32.dll",
            "WTSAPI32.dll",
            "HID.DLL",
            "UxTheme.dll",
            "ncrypt.dll",
            "USP10.dll",
            "WINSPOOL.DRV",
            "dxva2.dll",
            "CRYPTUI.dll",
            "urlmon.dll",
            "WINHTTP.dll",
            "credui.dll",
            "dhcpcsvc.DLL",
            "wevtapi.dll",
            "ESENT.dll",
            "iertutil.dll",
            "srvcli.dll",
            "netutils.dll",
            "DSROLE.DLL",
            "WKSCLI.DLL",
            "SAMCLI.DLL",
            "NTASN1.dll",
            "CRYPTSP.dll",
            "rsaenh.dll",
            "steamclient64.dll",
            "tier0_s64.dll",
            "vstdlib_s64.dll",
            "gpapi.dll",
            "GameOverlayRenderer64.dll",
            "WINNSI.DLL",
            "apphelp.dll",
            "clbcatq.dll",
            "nvldumdx.dll",
            "ros.dll",
            "DPAPI.dll",
            "xaudio2_7.dll",
            "nvwgf2umx.dll",
            "MSCTF.dll",
            "MessageBus.dll",
            "NLAapi.dll",
            "dhcpcsvc6.DLL",
            "dxcore.dll",
            "textinputframework.dll",
            "WINSTA.dll",
            "MMDevApi.dll",
            "mscms.dll",
            "ColorAdapterClient.dll",
            "Windows.UI.dll",
            "WindowManagementAPI.dll",
            "twinapi.appcore.dll",
            "DSREG.DLL",
            "msvcp110_win.dll",
            "mdnsNSP.dll",
            "rasadhlp.dll",
            "fwpuclnt.dll",
            "gfsdk_shadowlib.dll",
            "bink2w64.dll",
            "MF.dll",
            "MFCORE.DLL",
            "ksuser.dll",
            "MFPlat.DLL",
            "RTWorkQ.DLL",
            "msdmo.dll",
            "MFReadWrite.dll",
            "SwiftShaderD3D9_64.dll",
            "GFSDK_TXAA_AlphaResolve.win64.dll",
            "dxgi.dll",
            "AUDIOSES.DLL",
            "socialclub.dll",
            "Windows.Gaming.Input.dll",
            "XAudio2_8.dll",
            "Windows.Security.Authentication.Web.Core.dll",
            "xaudio2_9.DLL",
            "resourcepolicyclient.dll",
            "OneCoreCommonProxyStub.dll",
            "vaultcli.dll",
            "Windows.Web.dll",
            "Windows.Web.Http.dll",
            "ondemandconnroutehelper.dll",
            "firewallapi.dll",
            "fwbase.dll",
            "schannel.DLL",
            "mskeyprotect.dll",
            "ncryptsslp.dll",
            "kbdus.dll",
            "certenroll.dll",
            "certca.dll",
            "WLDAP32.dll",
            "DSPARSE.dll",
            "FWPolicyIOMgr.dll",
            "TextShaping.dll",
            "mlang.dll",
            "wdmaud.drv",
            "msacm32.drv",
            "MSACM32.dll",
            "midimap.dll",
            "graphics-hook64.dll",
            "MicrosoftAccountWAMExtension.dll",
            "explorerframe.dll",
            "OneCoreUAPCommonProxyStub.dll",
            "IconCodecService.dll",
            "LINKINFO.dll",
            "ntshrui.dll",
            "cscapi.dll",
            "policymanager.dll",
            "TaskFlowDataEngine.dll",
            "cdp.dll"
        };

        static void Main(string[] args)
        {
            Console.Title = "Scanner | Svvayyz#7153";

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

                        string fullString = "";
                        for (int i3 = 0; i3 < _trustedModules.Length; i3++)
                        {
                           fullString += _trustedModules[i3];
                        }

                        if (!fullString.Contains(module.ModuleName) && !module.ModuleName.EndsWith(".exe")) { Log(LogType.WARNING, $"fount a suspicious (unknown) module {module.ModuleName}"); }
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
