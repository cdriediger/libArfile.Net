﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArFile
{
    class Test
    {
        static void Main()
        {
            //AddFile();
            ReadFile();
            //AddData();
            //ReadArchive();
            //WriteTest();
            Console.ReadLine();
        }

        static public void AddData()
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            int f1 = archive.NewFile(@"C:\tmp\test.txt");
            string data = "1234567890";
            int c1 = archive.Metadata.GetChunk(data.Length);
            archive.WriteChunk(data, c1, f1);
            data = "ABCDEFGHIJKLMNOPGRSTUVWXYZ";
            int c2 = archive.Metadata.GetChunk(data.Length);
            archive.WriteChunk(data, c2, f1);
            archive.Dispose();
        }

        static public void AddFile()
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            string filePath = @"C:\tmp\test.pdf";
            int fileId = archive.NewFile(filePath);
            FileStream file = new FileStream(filePath, FileMode.Open);

            while (!(file.Position == file.Length))
            {
                int dataLength = archive.Metadata.defaultChunkSize;
                if (!(file.Length - file.Position >= dataLength))
                {
                    dataLength = unchecked((int)(file.Length - file.Position));
                }
                byte[] data = new byte[dataLength];
                file.Read(data, 0, dataLength);
                int chunkId = archive.Metadata.GetChunk(dataLength);
                archive.WriteChunk(data, chunkId, fileId);
            }
            file.Close();
            archive.Dispose();
        }

        static public void ReadFile()
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            string filePath = @"C:\tmp\restore.pdf";
            int fileId = 1;
            FileStream file = new FileStream(filePath, FileMode.Create);

            foreach (int chunkId in archive.Metadata.Files[fileId].Chunks)
            {
                file.Write(archive.ReadChunk(chunkId), 0, archive.Metadata.GetChunkLength(chunkId));
            }
            file.Close();
            archive.Dispose();
        }

        static public void ReadArchive()
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            string data = archive.ReadString(3);
            Logger.Warn($"Data: '{data}'");
        }

        static public void WriteTest()
        {
            FileStream ArchivFile = System.IO.File.Open(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile", FileMode.CreateNew, FileAccess.ReadWrite);
            ArchivFile.Seek(100, SeekOrigin.Begin);
            ArchivFile.Write(Encoding.ASCII.GetBytes("Data"), 0, 4);
            ArchivFile.Seek(0, SeekOrigin.Begin);
            ArchivFile.Write(Encoding.ASCII.GetBytes("Superblock"), 0, 10);
            ArchivFile.Flush();
            ArchivFile.Close();
        }
    }
}
