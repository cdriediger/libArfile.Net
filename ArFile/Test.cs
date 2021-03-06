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
            //AddFile(@"C:\tmp\test3.pdf");
            ReadFile(1);
            //DeleteFile(2);
            //ReclaimWhiteSpace();
            //AddData();
            //ReadArchive();
            //WriteTest();
            Console.ReadLine();
        }
        
        static public void AddFile(string filePath)
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
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
                archive.WriteChunk(data, fileId);
            }
            file.Close();
            archive.Dispose();
        }

        static public void ReadFile(int fileId)
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            string filePath = @"C:\tmp\restore.pdf";
            FileStream file = new FileStream(filePath, FileMode.Create);

            foreach (int chunkId in archive.Metadata.Files[fileId].Chunks)
            {
                byte[] chunkData = archive.ReadChunk(chunkId);
                file.Write(chunkData, 0, chunkData.Length);
            }
            file.Close();
            archive.Dispose();
        }

        static public void DeleteFile(int fileId)
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            archive.DeleteFile(fileId);
            archive.Dispose();
        }

        static public void ReclaimWhiteSpace()
        {
            Archive archive = new Archive(@"C:\Users\c.driediger\source\repos\ArFile\ArFile\testarchive.arfile");
            archive.ReclaimWhiteSpace();
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
