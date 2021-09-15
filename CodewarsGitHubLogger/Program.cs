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

            var responseStream = await httpClient.GetStringAsync($"{katasUrl}?page=0");
            var response = JsonSerializer.Deserialize<Kata>(responseStream);

            Console.WriteLine(response);
        }
    }

    class Kata
    {
        int totalPages;
        int totalItems;
        List<KataData> data;
    }

    class KataData
    {
        string id;
        string name;
        string slug;
        string completedAt;
        List<string> completedLanguages;
    }
}
