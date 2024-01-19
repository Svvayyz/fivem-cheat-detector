using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace fivemhackdetector.Classes
{
    internal class CXPress2
    {
        private const ushort CompressionFormatXpressHuff = 4;

        [DllImport("ntdll.dll")]
        private static extern uint RtlGetCompressionWorkSpaceSize(ushort compressionFormat,
            ref ulong compressBufferWorkSpaceSize, ref ulong compressFragmentWorkSpaceSize);

        [DllImport("ntdll.dll")]
        private static extern uint RtlDecompressBufferEx(ushort compressionFormat, byte[] uncompressedBuffer,
            int uncompressedBufferSize, byte[] compressedBuffer, int compressedBufferSize, ref int finalUncompressedSize,
            byte[] workSpace);

        public static byte[] Decompress(byte[] buffer, ulong decompressedSize)
        {
            var outBuf = new byte[decompressedSize];
            ulong compressBufferWorkSpaceSize = 0;
            ulong compressFragmentWorkSpaceSize = 0;

            var ret = RtlGetCompressionWorkSpaceSize(CompressionFormatXpressHuff, ref compressBufferWorkSpaceSize, ref compressFragmentWorkSpaceSize);
            if (ret != 0)
            {
                return null;
            }

            var workSpace = new byte[compressFragmentWorkSpaceSize];
            var dstSize = 0;

            ret = RtlDecompressBufferEx(CompressionFormatXpressHuff, outBuf, outBuf.Length, buffer, buffer.Length, ref dstSize, workSpace);

            return outBuf;
        }
    }
}
