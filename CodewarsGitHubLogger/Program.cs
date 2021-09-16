using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodewarsGitHubLogger
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            string codewarsUsername = "JoseDeFreitas"; // Environment.GetEnvironmentVariable("CODEWARS_USERNAME");
            string katasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string folderPath = "../Katas";

            Directory.CreateDirectory(folderPath);

            Stream responseJson = await httpClient.GetStreamAsync(katasUrl);
            Kata response = await JsonSerializer.DeserializeAsync<Kata>(responseJson);
            int numberOfPages = response.totalPages;

            for (int page = 0; page < numberOfPages; page++)
            {
                Stream responseStream = await httpClient.GetStreamAsync($"{katasUrl}?page={page}");
                Kata kataObject = await JsonSerializer.DeserializeAsync<Kata>(responseStream);

                foreach (var kata in kataObject.data)
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(folderPath, kata.slug));
                    }
                    catch (IOException exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            Console.WriteLine("All data was loaded successfully.");
        }
    }

    class Kata
    {
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public List<KataData> data { get; set; }
    }

    class KataData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string completedAt { get; set; }
        public List<string> completedLanguages { get; set; }
    }
}
