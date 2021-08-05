using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Middle_Earch_Arch05.LTAR
{
    public class FileBundle
    {
        public string file { get; set; }
        public UInt64 offsetData { get; set; }
        public UInt64 dataSize { get; set; }
        public UInt64 DecompressSize { get; set; }
    }

    public class DirBundle
    {
        public string dir { get; set; }
        public uint next_folder { get; set; }
        public uint back_folder { get; set; }
        public uint sub_files { get; set; }
    }

    public class Arch05Info
    {
        public int Version { get; set; }
        public int NameHeader { get; set; }
        public int Folders { get; set; }
        public int Files { get; set; }
    }

    public class FileBundles : List<FileBundle>
    {
        public FileBundle File { get; set; }
    }

    public class DirBundles : List<DirBundle>
    {
        public DirBundle Dir { get; set; }
    }

    public class Arch05File : BinaryReader
    {
        public Arch05File(Stream stream) : base(stream) { }

        public void ExtractArch05(DirBundles dirbundles, FileBundles filebundles, Arch05Info info)
        {
            string magic = Encoding.ASCII.GetString(ReadBytes(4));
            if(magic != "LTAR")
                throw new FormatException("Arch05: Bad magic Id.");
            info.Version = ReadInt32();
            info.NameHeader = ReadInt32();
            info.Folders = ReadInt32();
            info.Files = ReadInt32();

            BaseStream.Seek(0x1C, SeekOrigin.Current);

            byte[] names = ReadBytes(info.NameHeader);
            for(int i = 0; i< info.Files; i++)
            {
                uint seek = ReadUInt32();
                UInt64 offset = ReadUInt64();
                UInt64 size = ReadUInt64();
                UInt64 size_dess = ReadUInt64();
                int unk = ReadInt32();
                string fname = ReadString(names, seek);
                filebundles.Add(new FileBundle() { file = fname, offsetData = offset, dataSize = size, DecompressSize = size_dess });
            }

            for (int i = 0; i < info.Folders; i++)
            {
                uint seek = ReadUInt32();
                uint next_folder = ReadUInt32();
                uint back_folder = ReadUInt32();
                uint sub_files = ReadUInt32();
                string fname = string.Empty;
                if (seek == 0)
                    fname = "RootArch05";
                else
                    fname = ReadString(names, seek);
                dirbundles.Add(new DirBundle() { dir = fname, next_folder = next_folder, back_folder = back_folder, sub_files = sub_files });
            }
        }

        static string ReadString(byte[] stream, uint index)
        {
            string str = string.Empty;
            char ch;
            int i = 0;
            while ((int)(ch = (char)stream[index + i]) != 0)
            {
                str = str + ch;
                i++;
            }

            return str;
        }
    }
}
