using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTTP_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await Method_Beeline();
            await ProcessZipFileAsync();
        }

        //Вызов телефонов
        static async Task Method_Beeline()
        {
            string url = "https://beeline.kz/ru";
            using (var httpClient = new HttpClient())
            {
                var html = await httpClient.GetStringAsync(url);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var footerNode = htmlDoc.DocumentNode.SelectSingleNode("//footer") ??
                                 htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'footer')]"); 

                if (footerNode == null)
                {
                    Console.WriteLine("Не удалось найти элемент footer на странице.");
                    return;
                }

                var nodes = footerNode.SelectNodes(".//text()") ?? new HtmlNodeCollection(null);

                var phonePattern = new Regex(@"\+7 \(\d{3}\) \d{4} \d{3}", RegexOptions.Compiled);
                var shortPhonePattern = new Regex(@"\b\d{3,4}\b", RegexOptions.Compiled); 

                var phoneNumbers = new HashSet<string>();

                foreach (var node in nodes)
                {
                    var text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        var phoneMatches = phonePattern.Matches(text);
                        foreach (Match match in phoneMatches)
                        {
                            phoneNumbers.Add(match.Value);
                        }

                        var shortPhoneMatches = shortPhonePattern.Matches(text);
                        foreach (Match match in shortPhoneMatches)
                        {
                            var number = match.Value;
                            if (number == "116" || number == "3131")
                            {
                                phoneNumbers.Add(number);
                            }
                        }
                    }
                }

                Console.WriteLine("Телефоны расположены внизу сайта\n");
                foreach (var phoneNumber in phoneNumbers)
                {
                    Console.WriteLine(phoneNumber);
                }
            }
        }

        //ZIP файл https://github.com/mbaibatyr/SEP_221_NET/archive/refs/heads/master.zip	
        static async Task ProcessZipFileAsync()
        {
            string zipUrl = "https://github.com/mbaibatyr/SEP_221_NET/archive/refs/heads/master.zip";
            string zipFilePath = "master.zip";
            string extractPath = "SEP_221_NET-master";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(zipUrl);
                    response.EnsureSuccessStatusCode();
                    await using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }

                ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                string gitIgnorePath = Path.Combine(extractPath, ".gitignore");
                if (File.Exists(gitIgnorePath))
                {
                    string content = File.ReadAllText(gitIgnorePath);
                    Console.WriteLine(".gitignore содержимое:");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine(".gitignore не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
