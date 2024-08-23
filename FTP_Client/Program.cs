using FluentFTP;
using Renci.SshNet;
using System.Net;
using Renci.SshNet.Sftp;
using System.IO.Compression;
using System.Net.Mail;
using MimeKit;
using MailKit.Security;

namespace FTP_Client
{
    internal class Program
    {
        //https://filezilla.ru/download/FileZillaPortable_3.63.2.paf.exe
        static string urlPath = "https://beeline.kz/binaries/content/assets/public_offer/public_offer_ru.pdf";

        static string url = "ftp://ftp.dlptest.com/";
        static string user = "dlpuser";
        static string password = "rNrKYTX9g7z3RgJRmxWuGHbeu";
        static int ftpPort = 21;
        static string Host = "test.rebex.net";
        static int Port = 22;
        static string Username = "demo";
        static string Password = "password";

        //2 задание
        static void FTP()
        {
            //string[] filePaths =
            //{
            //    @"C:\Users\dinas\Desktop\sample1.txt",
            //    @"C:\Users\dinas\Desktop\sample2.txt",
            //    @"C:\Users\dinas\Desktop\sample3.txt",
            //    @"C:\Users\dinas\Desktop\sample4.txt"
            //};

            //string archivePath = @"C:\Users\dinas\Desktop\archive.zip";
            //string downloadPath = @"C:\Users\dinas\Desktop\archive.zip";
            //string SAMPLE1234 = @"C:\Users\dinas\Desktop\files\";

            //try
            //{
            //    using (ZipArchive zip = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            //    {
            //        foreach (var file in filePaths)
            //        {
            //            zip.CreateEntryFromFile(file, Path.GetFileName(file));
            //        }
            //    }
            //    Console.WriteLine("Архив создан: " + archivePath);

            //    using (FtpClient client = new FtpClient())
            //    {
            //        client.Host = url;
            //        client.Credentials = new NetworkCredential(user, password);
            //        client.Connect();

            //        client.UploadFile(archivePath, "/archive.zip");
            //        Console.WriteLine("Архив загружен на FTP-сервер: /archive.zip");
            //    }

            //    using (FtpClient client = new FtpClient())
            //    {
            //        client.Host = url;
            //        client.Credentials = new NetworkCredential(user, password);
            //        client.Connect();

            //        client.DownloadFile(downloadPath, "/archive.zip");
            //        Console.WriteLine("Архив скачан с FTP-сервера: " + downloadPath);
            //    }

            //    if (!Directory.Exists(SAMPLE1234))
            //    {
            //        Directory.CreateDirectory(SAMPLE1234);
            //    }
            //    ZipFile.ExtractToDirectory(downloadPath, SAMPLE1234);
            //    Console.WriteLine("Архив разархивирован в папку: " + SAMPLE1234);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Произошла ошибка: " + ex.Message);
            //}

            //*3 задание*
            FtpClient client = new FtpClient();
            client.Host = url;
            client.Credentials = new NetworkCredential(user, password);
            client.Connect();

            client.UploadFile(urlPath, "/08/23/ftp_bee.pdf");
            Console.WriteLine("Файл загружен на FTP-сервер: /ftp_bee.pdf");
        }

        //3 задание
        static void SFTP()
        {
            string RemoteDestinationFilename = "/pub/SFTP_bee.pdf";

            using (var sftp = new SftpClient(Host, Port, Username, Password))
            using (var httpClient = new HttpClient())
            {
                sftp.Connect();

                var response = httpClient.GetAsync(urlPath).Result;

                if (response.IsSuccessStatusCode)
                {
                    using (var fileStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        sftp.UploadFile(fileStream, RemoteDestinationFilename);
                        Console.WriteLine($"Файл успешно загружен на SFTP-сервер: {RemoteDestinationFilename}");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка при загрузке файла с URL: " + response.StatusCode);
                }
                sftp.Disconnect();
            }
        }

        //8 задание
        static void SFTP2()
        {
            string fileName = "sample1.txt";

            string imapServer = "imap.mail.ru";
            int smtpPort = 993;
            string smtpUsername = "*****";
            string smtpPassword = "*******";
            string recipientEmail = "dinash6145@mail.ru";

            string ftpUploadPath = "/";

            string tempZipPath = "temp.zip";

            using (var sftpClient = new SftpClient(Host, Port, Username, Password))
            {
                sftpClient.Connect();

                using (var stream = new MemoryStream())
                {
                    sftpClient.DownloadFile(fileName, stream);
                    stream.Position = 0;

                    using (var archiveStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                        {
                            var archiveEntry = archive.CreateEntry(fileName);
                            using (var entryStream = archiveEntry.Open())
                            {
                                stream.CopyTo(entryStream);
                            }
                        }

                        archiveStream.Position = 0;

                        var message = new MailMessage
                        {
                            From = new MailAddress(smtpUsername, "Your Name"),
                            Subject = "Your archived file",
                            Body = "Please find the attached archived file.",
                            IsBodyHtml = true
                        };
                        message.To.Add(recipientEmail);

                        var attachment = new Attachment(archiveStream, fileName + ".zip");
                        message.Attachments.Add(attachment);

                        using (var smtpClient = new SmtpClient(imapServer, smtpPort))
                        {
                            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                            smtpClient.EnableSsl = true;
                            smtpClient.Send(message);
                        }
                    }
                }
                sftpClient.Disconnect();
            }

            string ftpFilePath = ftpUploadPath + "temp.zip";

            var ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{url}{ftpFilePath}"));
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.Credentials = new NetworkCredential(user, password);

            using (var fileStream = new FileStream(tempZipPath, FileMode.Open, FileAccess.Read))
            {
                using (var ftpStream = ftpRequest.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
            }

            File.Delete(tempZipPath);
        }

        static void Main(string[] args)
        {
            //FTP();
            //SFTP();
            SFTP2();

            //3 задание
            //WebClient webClient = new WebClient();
            //webClient.DownloadFile(urlPath, @"C:\Users\dinas\Desktop\beeline.pdf");
            //Console.WriteLine("Файл сохранен");
        }

    }
}