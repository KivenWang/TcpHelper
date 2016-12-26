using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TcpHelper;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //TcpListenerHelper listener = new TcpListenerHelper("127.0.0.1", 3699);
            TcpListenerHelper listener = new TcpListenerHelper("0.0.0.0", 3699);
            //TcpListenerHelper listener = new TcpListenerHelper("192.168.100.200", 3688);
            listener.Start();
            Task task = new Task(DoWork, listener);
            task.Start();
            Console.Read();

        }

        static void DoWork(object state)
        {
            TcpListenerHelper tlistener = (TcpListenerHelper)state;
            tlistener.Listen();//监听
            
            while (tlistener.WaitForConnect())//等待直到监听到了连接
            {
                try
                {
                    string firstMessage = "";
                    while (!string.IsNullOrEmpty((firstMessage = tlistener.ReadMessage())))
                    {
                        Console.WriteLine(firstMessage);
                        if (firstMessage.ToLower() == "filebak".ToLower())
                        {
                            tlistener.SendMessage("filebakok");
                            #region 文件备份
                            string filepath = Path.Combine(Environment.CurrentDirectory, "FileBak\\" + tlistener.ReadMessage()).ToString();
                            tlistener.ReceiveFile(filepath);
                            if (tlistener.CalcFileHash(filepath) == tlistener.ReadMessage())
                            {
                                tlistener.SendMessage("ok");
                                Console.WriteLine("接收到文件：{0}", filepath);
                            }
                            else
                            {
                                tlistener.SendMessage("wrong");
                                Console.WriteLine("接收失败");
                            }
                            #endregion
                        }
                        else if (firstMessage.ToLower() == "DBBak".ToLower())
                        {
                            #region 数据库备份
                            tlistener.SendMessage("dbbakok");
                            string filename = tlistener.ReadMessage();
                            string filepath = Path.Combine(System.Environment.CurrentDirectory, "DBBak") + "\\" + filename;
                            //接收文件
                            tlistener.ReceiveFile(filepath);
                            //验证hash值
                            string hash = tlistener.ReadMessage();
                            if (hash == tlistener.CalcFileHash(filepath))
                            {
                                tlistener.SendMessage("ok");
                                Console.WriteLine("接收到文件：{0}", filepath);
                            }
                            else
                            {
                                tlistener.SendMessage("wrong");
                                Console.WriteLine("接收失败");
                            }
                            #endregion
                        }
                    }
                }
                catch
                {
                }

                tlistener.Listen();//监听下一个连接
            }
        }
    }
}
