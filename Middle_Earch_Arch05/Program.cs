using System.IO;
using Middle_Earch_Arch05.LTAR;

namespace Middle_Earch_Arch05
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            FileAttributes attr = File.GetAttributes(args[0]);

            string directory = Path.GetDirectoryName(args[0]);
            string filename = Path.GetFileName(args[0]);
            string withoutext = Path.GetFileNameWithoutExtension(args[0]);
            string ext = Path.GetExtension(args[0]);

            if (!attr.HasFlag(FileAttributes.Directory))
            {
                if (ext == ".arch05")
                {
                    using (Arch05File rd = new Arch05File(File.OpenRead(args[0])))
                    {
                        DirBundles dirs = new DirBundles();
                        FileBundles files = new FileBundles();
                        Arch05Info info = new Arch05Info();
                        rd.ExtractArch05(dirs, files, info);
                        foreach (var dir in dirs)
                        {
                            string path = dir.dir;
                            if (path == "RootArch05")
                            {
                                if (!Directory.Exists(directory + "\\" + filename + ".ex"))
                                    Directory.CreateDirectory(directory + "\\" + filename + ".ex");
                            }
                            else
                            {
                                if (!Directory.Exists(directory + "\\" + filename + ".ex\\" + path))
                                    Directory.CreateDirectory(directory + "\\" + filename + ".ex\\" + path);
                            }

                            for (int i = 0; i < dir.sub_files; i++)
                            {
                                var wt = new BinaryWriter(File.Create(directory + "\\" + filename + ".ex\\" + path + "\\" + files[i].file));
                                if (files[i].DecompressSize > 0xFFFF)
                                {
                                    int j = 0;
                                    rd.BaseStream.Seek((long)files[i].offsetData, SeekOrigin.Begin);
                                    while (j < (int)files[i].DecompressSize)
                                    {
                                        int zsizeData = rd.ReadInt32();
                                        int sizeData = rd.ReadInt32();
                                        byte[] blockdata = rd.ReadBytes(zsizeData);
                                        Arch05Decompress ds = new Arch05Decompress(blockdata);
                                        if (zsizeData == sizeData)
                                            wt.Write(blockdata);
                                        else
                                            wt.Write(ds.OutDataDecompress());
                                        ReadNull(rd, (int)rd.BaseStream.Position);
                                        j += sizeData;
                                    }
                                    wt.Close();
                                }
                                else
                                {
                                    rd.BaseStream.Seek((long)files[i].offsetData, SeekOrigin.Begin);
                                    int zsizeData = rd.ReadInt32();
                                    int sizeData = rd.ReadInt32();
                                    byte[] blockdata = rd.ReadBytes(zsizeData);
                                    Arch05Decompress ds = new Arch05Decompress(blockdata);
                                    if (zsizeData == sizeData)
                                        wt.Write(blockdata);
                                    else
                                        wt.Write(ds.OutDataDecompress());
                                    wt.Close();
                                }
                            }

                            files.RemoveRange(0, (int)dir.sub_files);
                        }
                    }
                }
            }
        }

        static void WriteNull(BinaryWriter wt, long pos, byte varible)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((pos + i) % 4 != 0)
                    wt.Write(varible);
                else
                    break;
            }
        }

        static void ReadNull(Arch05File rd, int pos)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((pos + i) % 4 != 0)
                    rd.ReadByte();
                else
                    break;
            }
        }
    }
}
