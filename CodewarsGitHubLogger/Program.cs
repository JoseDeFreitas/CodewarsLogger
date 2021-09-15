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

        static void Main(string[] args)
        {
            string folderPath = "../Katas/";

            Directory.CreateDirectory(folderPath);

            Console.WriteLine(GetKatas(0));
        }

        static async Task<string> GetKatas(int pageNumber)
        {
            var response = await httpClient.GetStringAsync($"{userUrl}/code-challenges/completed?page={pageNumber}");
        }
    }
}
