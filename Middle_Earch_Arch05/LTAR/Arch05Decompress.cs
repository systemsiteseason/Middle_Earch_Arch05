using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace Middle_Earch_Arch05.LTAR
{
    public class Arch05Decompress : MemoryStream
    {
        public Arch05Decompress(byte[] blockdata) : base(blockdata) { }

        public byte[] OutDataDecompress()
        {
            var outputStream = new MemoryStream();
            using (var compressedStream = this)
            using (var inputStream = new InflaterInputStream(compressedStream))
            {
                inputStream.CopyTo(outputStream);
                outputStream.Position = 0;
                return outputStream.ToArray();
            }
        }
    }
}
