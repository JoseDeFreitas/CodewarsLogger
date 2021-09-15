using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CodewarsGitHubLogger
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();
        static string codewarsUsername = Environment.GetEnvironmentVariable("CODEWARS_USERNAME");
        static string userUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}";

        static async Task Main(string[] args)
        {
            string folderPath = "../Katas/";

            Directory.CreateDirectory(folderPath);

            var response = await httpClient.GetStringAsync($"{userUrl}/code-challenges/completed?page=0");

            Console.WriteLine(response);
        }
    }
}
