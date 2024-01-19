using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using FastHashes;

namespace fivemhackdetector.Classes
{
    public class CPrefetchFile
    {
        private byte[] pBytes = { };

        public bool bSuccess = true;
        public string szPath = "";
        public string szExecutableName = "";
        public string szHash = "";
        public string szCheatName = "none";
        public List<string> mFileNames = new List<string>();

        public CPrefetchFile(string path) {
            try
            {
                pBytes = Load(path);
            }
            catch (Exception)
            {
                bSuccess = false;
            }

            Initialize();
        }

        private byte[] Load(string str)
        {
            byte[] bytes = File.ReadAllBytes(str);

            string sig = Encoding.ASCII.GetString(bytes, 0, 3);

            if (sig == "MAM")
            {
                uint size = BitConverter.ToUInt32(bytes, 4);
                byte[] compressed = bytes.Skip(8).ToArray();

                bytes = CXPress2.Decompress(compressed, size);
            }

            int siggy = BitConverter.ToInt32(bytes, 4);

            if (siggy != 0x41434353)
                throw new Exception("wrong sig");

            return bytes;
        }
        private string BruteforceDisk(string str)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string[] splitted = str.Split('\\');

                string path = str.Replace("\\" + splitted[0] + splitted[1] + "\\", drive.Name);

                if (File.Exists(path))
                {
                    return path;
                }
            }

            return str;
        }
        private void Initialize()
        {
            try
            {
                byte[] headerBytes = new byte[84];
                Buffer.BlockCopy(pBytes, 0, headerBytes, 0, headerBytes.Length);

                string tmpName = Encoding.Unicode.GetString(headerBytes, 16, 60);
                string name = tmpName.Substring(0, tmpName.IndexOf('\0')).Trim();

                int fileSize = BitConverter.ToInt32(headerBytes, 12);

                szExecutableName = name;

                byte[] infoBytes = new byte[224];
                Buffer.BlockCopy(pBytes, headerBytes.Length, infoBytes, 0, infoBytes.Length);

                int stringsOffset = BitConverter.ToInt32(infoBytes, 16);
                int stringsSize = BitConverter.ToInt32(infoBytes, 20);

                byte[] stringsBytes = new byte[stringsSize];
                Buffer.BlockCopy(pBytes, stringsOffset, stringsBytes, 0, stringsSize);

                string tmpFilenames = Encoding.Unicode.GetString(stringsBytes);
                string[] filenamesArray = tmpFilenames.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

                List<string> filenames = filenamesArray.ToList();
                foreach (string filename in filenames)
                {
                    if (Path.GetFileNameWithoutExtension(filename).EndsWith(
                        Path.GetFileNameWithoutExtension(name)
                    ))
                    {
                        szPath = BruteforceDisk(filename);
                    }

                    if (filename.ToLower().EndsWith("settings.cock"))
                        szCheatName = "redengine";
                }

                mFileNames = filenames;

                if (fileSize < 200000)
                {
                    const int bytesCount = 4096;

                    FarmHash64 hash = new FarmHash64();
                    BufferedStream stream = new BufferedStream(File.OpenRead(szPath), bytesCount);

                    byte[] bytes = { };
                    stream.Read(bytes, 0, bytesCount);

                    szHash = Convert.ToBase64String(hash.ComputeHash(bytes));
                }
            }
            catch (Exception)
            {
                bSuccess = false;
            }
        }
    }

    internal class CPrefetch
    {
        private List<CPrefetchFile> mPrefetchFiles = new List<CPrefetchFile>();

        public CPrefetch()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach (string path in Directory.GetFiles("C:\\Windows\\Prefetch"))
            {
                CPrefetchFile file = new CPrefetchFile(path);

                if (!file.bSuccess)
                    continue;

                mPrefetchFiles.Add(file);
            }
        }
        public List<CPrefetchFile> GetFiles()
        {
            return mPrefetchFiles;
        }
    }
}
