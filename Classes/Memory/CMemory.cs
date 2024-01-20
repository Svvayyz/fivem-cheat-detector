using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace fivemhackdetector.Classes
{
    public class CMemoryString
    {
        public byte[] szStr;
        public int iLenght;
        public long uAddress;

        public CMemoryString(byte[] str, int len, long addr)
        {
            szStr = str;
            iLenght = len;
            uAddress = addr;
        }
    }

    internal class CMemory
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        private const int MEMORY_TYPE_IMAGE = 0x1000000;
        private const int MEMORY_TYPE_PRIVATE = 0x20000;
        private const int MEMORY_TYPE_MAPPED = 0x40000;

        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;

        private bool[] charIsPrintable =
                   {
                        false, false, false, false, false, false, false, false, false, true, true, false, false, true, false, false, /* false - true5 */ // TAB, LF and CR are printable
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true6 - 3true */
                        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, /* ' ' - '/' */
                        true, true, true, true, true, true, true, true, true, true, /* 'false' - '9' */
                        true, true, true, true, true, true, true, /* ':' - '@' */
                        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, /* 'A' - 'Z' */
                        true, true, true, true, true, true, /* '[' - '`' */
                        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, /* 'a' - 'z' */
                        true, true, true, true, false, /* '{' - true27 */ // DEL is not printable
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true28 - true43 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true44 - true59 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true6false - true75 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true76 - true9true */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* true92 - 2false7 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* 2false8 - 223 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, /* 224 - 239 */
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false /* 24false - 255 */
                    };

        private Process pProcess;
        private IntPtr pHandle;

        public CMemory(Process process)
        {
            pProcess = process;
            pHandle = OpenProcess(
                PROCESS_QUERY_INFORMATION | 
                PROCESS_VM_OPERATION | 
                PROCESS_VM_READ, 
                false, 
                process.Id
            );
        }
        ~CMemory()
        {
            CloseHandle(pHandle);
        }

        public MEMORY_BASIC_INFORMATION GetRegionInfo(IntPtr address)
        {
            MEMORY_BASIC_INFORMATION info = new MEMORY_BASIC_INFORMATION();

            VirtualQueryEx(pHandle, address, out info, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

            return info;
        }

        public Dictionary<int, string> GetMemoryRegions()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            IntPtr addr = pProcess.MainModule.BaseAddress;
            IntPtr end = pProcess.MainModule.BaseAddress + pProcess.MainModule.ModuleMemorySize;

            while (addr.ToInt64() < end.ToInt64())
            {
                MEMORY_BASIC_INFORMATION info = GetRegionInfo(addr);

                Console.WriteLine(info.AllocationBase);

                addr = new IntPtr(addr.ToInt64() + info.RegionSize.ToInt64());
            }

            return result;
        }

        public List<CMemoryString> mResult;
        public void ProcessChunk(int chunkSize, IntPtr startAddress, byte[] buffer, int bytesRead, int min_lenght)
        {
            ReadProcessMemory((int)pHandle, startAddress.ToInt64(), buffer, buffer.Length, ref bytesRead);

            byte[] displayBytes = new byte[chunkSize];
            int len = 0;

            bool[] printables = { true, true };
            byte[] bytes = { 0x0, 0x0 };

            for (int i = 0; i < buffer.Length; i++)
            {
                byte b = buffer[i];

                bool printable = false;

                printable = charIsPrintable[b];

                if (printables[1] && printables[0] && printable)
                {
                    if (len < 2048)
                        displayBytes[len] = b;

                    len++;
                }
                else if (printables[1] && printables[0] && !printable)
                {
                    if (len >= min_lenght)
                    {
                        goto createResult;
                    }
                    else if (b == 0)
                    {
                        len = 1;
                        displayBytes[0] = bytes[0];
                    }
                    else
                    {
                        len = 0;
                    }
                }
                else if (printables[1] && !printables[0] && printable)
                {
                    if (bytes[0] == 0)
                    {
                        if (len < 2048)
                            displayBytes[len] = b;

                        len++;
                    }
                }
                else if (printables[1] && !printables[0] && !printable)
                {
                    if (len >= min_lenght)
                    {
                        goto createResult;
                    }
                    else
                    {
                        len = 0;
                    }
                }
                else if (!printables[1] && printables[0] && printable)
                {
                    if (len >= min_lenght + 1)
                    {
                        len--;

                        goto createResult;
                    }
                    else
                    {
                        len = 2;
                        displayBytes[0] = bytes[0];
                        displayBytes[1] = b;
                    }
                }
                else if (!printables[1] && !printables[0] && printable)
                {
                    if (len < 2048)
                        displayBytes[len] = b;

                    len++;
                }

                goto afterCreateResult;

            createResult:

                bool isWide = printables[0] == printable;
                int lenght = isWide ? len * 2 : len;
                int bias = 0;

                if (printable)
                {
                    bias = 1;
                }

                long addr = startAddress.ToInt64() + i - bias - lenght;

                mResult.Add(new CMemoryString(displayBytes, lenght, addr));

            afterCreateResult:

                printables[1] = printables[0];
                printables[0] = printable;

                bytes[1] = bytes[0];
                bytes[0] = b;
            }
        }

        public List<CMemoryString> GetStrings()
        {
            mResult = new List<CMemoryString>();

            IntPtr startAddress = pProcess.MainModule.BaseAddress;
            IntPtr endAddress = IntPtr.Add(startAddress, pProcess.MainModule.ModuleMemorySize);

            const int chunkSize = 1024 * 4;

            byte[] buffer = new byte[chunkSize];
            int bytesRead = 0;

            int min_lenght = 4;

            while (startAddress.ToInt64() < endAddress.ToInt64())
            {
                ProcessChunk(chunkSize, startAddress, buffer, bytesRead, min_lenght);

                startAddress = IntPtr.Add(startAddress, chunkSize);
            }

            return mResult;
        }
    }
}
