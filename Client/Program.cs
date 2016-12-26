using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TcpHelper;

namespace Client
{
    struct FileInfo
    {
        public string Name;
        public string FullPath;
    }

    class Program
    {
        static void Main(string[] args)
        {
            TcpClientHelper client = new TcpClientHelper("127.0.0.1", 3699);
            client.Start();
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(FileBackup, client);
            task.Start();
            Console.Read();
        }

        static void FileBackup(object arg)
        {
            TcpClientHelper client = (TcpClientHelper)arg;

            //获取需备份的文件
            //DataTable dt = this._oFileWatch.GetBackupFiles();
            List<FileInfo> files = new List<FileInfo>();
            files.Add(new FileInfo() { Name = "log.txt", FullPath = @"C:\log.txt" });
            files.Add(new FileInfo() { Name = "dotNetFx40_Full_x86_x64.rar", FullPath = @"C:\dotNetFx40_Full_x86_x64.rar" });

            foreach (var file in files)
            {
                client.SendMessage("FileBak");
                //Thread.Sleep(10000);

                if (client.ReadMessage().ToLower() == "filebakok")
                {
                    client.SendMessage(file.Name);
                    client.SendFile(file.FullPath);
                    client.SendMessage(client.CalcFileHash(file.FullPath));

                    if (client.ReadMessage().ToLower() == "ok")
                    {
                        Console.WriteLine("备份文件【" + file.FullPath + "】成功");
                    }
                    else
                    {
                        Console.WriteLine("备份文件【" + file.FullPath + "】失败。");
                    }
                }

            }
        }
    }
}
