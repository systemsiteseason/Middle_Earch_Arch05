using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middle_Earch_Arch05.LTAR
{
    public class Arch05Compress : MemoryStream
    {
        public Arch05Compress(byte[] buffer) : base(buffer) { }

        public byte[] OutDataCompress()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZlibStream zip = new ZlibStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(this.ToArray(), 0, (int)this.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
