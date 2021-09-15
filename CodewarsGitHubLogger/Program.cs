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
        static string codewarsUsername = "JoseDeFreitas"; // Environment.GetEnvironmentVariable("CODEWARS_USERNAME");
        static string katasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";

        static async Task Main(string[] args)
        {
            string folderPath = "../Katas/";

            Directory.CreateDirectory(folderPath);

            var responseJson = await httpClient.GetStreamAsync($"{katasUrl}?page=0");
            Kata response = await JsonSerializer.DeserializeAsync<Kata>(responseJson);

            Console.WriteLine(response);
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
