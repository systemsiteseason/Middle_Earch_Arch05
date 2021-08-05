using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace Middle_Earch_Arch05.LTAR
{
    public class Arch05Decompress : MemoryStream
    {
        public Arch05Decompress(byte[] blockdata) : base(blockdata) { }

        public byte[] OutDataDecompress()
        {
            using (ZlibStream decompressor = new Ionic.Zlib.ZlibStream(this, CompressionMode.Decompress))
            {
                int read = 0;
                var buffer = new byte[1024 * 4];

                using (MemoryStream output = new MemoryStream())
                {
                    while ((read = decompressor.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                    return output.ToArray();
                }
            }
        }
    }
}
