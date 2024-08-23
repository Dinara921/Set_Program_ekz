using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace UDP_Client
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string[] filePaths = 
                {
                @"C:\Users\dinas\Desktop\sample1.txt",
                @"C:\Users\dinas\Desktop\sample2.txt",
                @"C:\Users\dinas\Desktop\sample3.txt",
                @"C:\Users\dinas\Desktop\sample4.txt"
            };
            string serverIP = "127.0.0.1";
            int serverPort = 8001;

            foreach (string filePath in filePaths)
            {
                UDP.SendFile(filePath, serverIP, serverPort);
            }
        }

        class UDP
        {
            public static void SendFile(string filePath, string serverIP, int serverPort)
            {
                try
                {
                    using (UdpClient udpClient = new UdpClient())
                    {
                        string fileName = Path.GetFileName(filePath);
                        byte[] fileNameBytes = Encoding.UTF8.GetBytes("FILE:" + fileName);
                        udpClient.Send(fileNameBytes, fileNameBytes.Length, serverIP, serverPort);

                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        udpClient.Send(fileBytes, fileBytes.Length, serverIP, serverPort);

                        Console.WriteLine("Отправлен файл: " + fileName);

                        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                        byte[] responseBytes = udpClient.Receive(ref serverEndPoint);
                        string response = Encoding.UTF8.GetString(responseBytes);
                        Console.WriteLine("Ответ сервера: " + response);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке файла: " + ex.Message);
                }
            }
        }
    }
}
