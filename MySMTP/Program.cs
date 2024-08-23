using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using System.IO.Compression;
using System.Net.Mail;

namespace MySMTP
{
    internal class Program
    {
        //https://help.mail.ru/mail/mailer/popsmtp

        static string imapServer = "imap.mail.ru";
        static int port = 993;
        static string email = "***";
        static string password = "***";
        static void Main(string[] args)
        {
            //MailSend.Send();
            //MailSend.GetImapTerm();
            MailSend.GetEmail();
        }

        public class MailSend
        {
            //5 задание
            public static void Send()
            {
                try
                {
                    string[] filePaths =
                    {
                        @"C:\Users\dinas\Desktop\sample1.txt",
                        @"C:\Users\dinas\Desktop\sample2.txt",
                        @"C:\Users\dinas\Desktop\sample3.txt",
                        @"C:\Users\dinas\Desktop\sample4.txt"
                    };

                    string archivePath = @"C:\Users\dinas\Desktop\archive.zip";
                    string downloadPath = @"C:\Users\dinas\Desktop\archive.zip";
                    string SAMPLE1234 = @"C:\Users\dinas\Desktop\files\";

                    try
                    {
                        using (ZipArchive zip = ZipFile.Open(archivePath, ZipArchiveMode.Create))
                        {
                            foreach (var file in filePaths)
                            {
                                zip.CreateEntryFromFile(file, Path.GetFileName(file));
                            }
                        }
                        Console.WriteLine("Архив создан: " + archivePath);


                        MailMessage mail = new MailMessage();
                        mail.IsBodyHtml = true;
                        mail.From = new MailAddress("dinash6145@mail.ru");
                        mail.To.Add("dinash6145@mail.ru");
                        //mail.Bcc.Add("murat_b@mail.ru");
                        mail.Subject = "subject sample";
                        mail.Body = "<h1>body sample </h1>";
                        Attachment attachment = new Attachment(archivePath);

                        mail.Attachments.Add(attachment);
                        using (System.Net.Mail.SmtpClient server = new System.Net.Mail.SmtpClient("smtp.mail.ru", 587))
                        {
                            server.UseDefaultCredentials = false;
                            server.Credentials = new System.Net.NetworkCredential("*********", "***********");
                            server.EnableSsl = true;
                            server.DeliveryMethod = SmtpDeliveryMethod.Network;
                            server.Send(mail);
                            server.Dispose();
                            Console.WriteLine("Send");
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }

                }
                catch (Exception err)
                {
                    //logger.Error(err.Message);
                    //return "error: " + err.Message;
                }
            }

            //6задание
            public static void GetImapTerm()
            {

                string searchTerm = "sample";

                using (var client = new ImapClient())
                {
                    client.Connect(imapServer, port, true);

                    client.Authenticate(email, password);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    var query = SearchQuery.SubjectContains(searchTerm).Or(SearchQuery.BodyContains(searchTerm));
                    var results = inbox.Search(query);

                    foreach (var uniqueId in results)
                    {
                        var message = inbox.GetMessage(uniqueId);

                        Console.WriteLine($"Найдено письмо с темой: {message.Subject}");
                        inbox.AddFlags(uniqueId, MessageFlags.Seen, true);

                        var targetFolder = client.GetFolder("Inbox/Динара");
                        if (targetFolder == null)
                        {
                            targetFolder = client.GetFolder(client.PersonalNamespaces[0]).Create("Inbox/Динара", true);
                        }
                        inbox.MoveTo(uniqueId, targetFolder);
                    }

                    client.Disconnect(true);
                }

                Console.WriteLine("Задача выполнена.");
            }

            //10 задание
            public static void GetEmail()
            {
                using (var client = new ImapClient())
                {
                    client.Connect(imapServer, port, SecureSocketOptions.SslOnConnect);

                    client.Authenticate(email, password);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    var uids = inbox.Search(SearchQuery.All).Take(100);

                    foreach (var uid in uids)
                    {
                        var message = inbox.GetMessage(uid);
                        Console.WriteLine($"Subject: {message.Subject}");
                    }

                    client.Disconnect(true);
                }
            }

        }
    }
}