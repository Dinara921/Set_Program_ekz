using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace UDP_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UDP.Listen();
        }
    }

    class UDP
    {
        public static void Listen()
        {
            int PORT = 8001;
            UdpClient udpClient = new UdpClient(PORT);
            Console.WriteLine("Сервер слушает на порту " + PORT);

            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, PORT);
                byte[] receivedBytes = udpClient.Receive(ref clientEndPoint);
                string receivedData = Encoding.UTF8.GetString(receivedBytes);

                if (receivedData.StartsWith("FILE:"))
                {
                    string fileName = receivedData.Substring(5);
                    ReceiveFile(udpClient, clientEndPoint, fileName);
                }
                else
                {
                    Console.WriteLine("Получены неожиданные данные: " + receivedData);
                }
            }
        }

        private static void ReceiveFile(UdpClient client, IPEndPoint clientEndPoint, string fileName)
        {
            Console.WriteLine("Получение файла: " + fileName);

            try
            {
                byte[] fileBytes = client.Receive(ref clientEndPoint);
                File.WriteAllBytes(fileName, fileBytes);

                Console.WriteLine("Файл получен и сохранен: " + fileName);

                byte[] responseBytes = Encoding.UTF8.GetBytes("Файл успешно сохранен: " + fileName);
                client.Send(responseBytes, responseBytes.Length, clientEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении файла: " + ex.Message);
            }
        }
    }
}
