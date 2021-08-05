using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Middle_Earch_Arch05.LTAR;

namespace Middle_Earch_Arch05
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = Path.GetDirectoryName(args[0]);
            string filename = Path.GetFileName(args[0]);
            string withoutext = Path.GetFileNameWithoutExtension(args[0]);
            string ext = Path.GetExtension(args[0]);

            using (Arch05File rd = new Arch05File(File.OpenRead(args[0])))
            {
                DirBundles dirs = new DirBundles();
                FileBundles files = new FileBundles();
                Arch05Info info = new Arch05Info();
                rd.ExtractArch05(dirs, files, info);
                foreach(var dir in dirs)
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

                    for(int i = 0; i < dir.sub_files; i++)
                    {
                        var wt = new BinaryWriter(File.Create(directory + "\\" + filename + ".ex\\" + path + "\\" + files[i].file));
                        if(files[i].DecompressSize > 0xFFFF)
                        {
                            int j = 0;
                            while(j < (int)files[i].DecompressSize)
                            {
                                rd.BaseStream.Seek((long)files[i].offsetData, SeekOrigin.Begin);
                                int zsizeData = rd.ReadInt32();
                                int sizeData = rd.ReadInt32();
                                Arch05Decompress ds = new Arch05Decompress(rd.ReadBytes(zsizeData));
                                wt.Write(ds.OutDataDecompress());
                                j += sizeData;
                            }
                            wt.Close();
                        }
                        else
                        {
                            rd.BaseStream.Seek((long)files[i].offsetData, SeekOrigin.Begin);
                            int zsizeData = rd.ReadInt32();
                            int sizeData = rd.ReadInt32();
                            Arch05Decompress ds = new Arch05Decompress(rd.ReadBytes(zsizeData));
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
