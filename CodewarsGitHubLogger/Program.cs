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
            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";
            string mainFolderPath = "../Katas";

            Directory.CreateDirectory(mainFolderPath);

            // Response used to get the total number of pages available
            Stream mainResponseJson = await httpClient.GetStreamAsync(completedKatasUrl);
            KataCompleted mainResponseObject = await JsonSerializer.DeserializeAsync<KataCompleted>(mainResponseJson);
            int numberOfPages = mainResponseObject.totalPages;

            int numberOfExceptions = 0;

            for (int page = 0; page < numberOfPages; page++)
            {
                // Response used to get the information of all the katas in the specified page
                Stream responseStream = await httpClient.GetStreamAsync($"{completedKatasUrl}?page={page}");
                KataCompleted kataObject = await JsonSerializer.DeserializeAsync<KataCompleted>(responseStream);

                foreach (var kata in kataObject.data)
                {
                    // Response used to get the description of the kata
                    Stream responseKataInfo = await httpClient.GetStreamAsync($"{kataInfoUrl}{kata.id}");
                    KataInfo kataInfoObject = await JsonSerializer.DeserializeAsync<KataInfo>(responseKataInfo);

                    string kataFolder = Path.Combine(mainFolderPath, kata.slug);

                    try
                    {
                        Directory.CreateDirectory(kataFolder);
                    }
                    catch (IOException exception)
                    {
                        Console.WriteLine(exception);
                        numberOfExceptions++;
                        continue;
                    }

                    string[] content =
                    {
                        $"# [{kata.name}]({kataInfoUrl}{kata.id})\n",
                        $"**Completed at:** {kata.completedAt}\n",
                        $"**Completed languages:** {string.Join(", ", kata.completedLanguages)}\n",
                        $"## Description\n{kataInfoObject.description}"
                    };

                    await File.WriteAllLinesAsync(Path.Combine(kataFolder, "README.md"), content);
                }
            }

            if (numberOfExceptions == 0)
                Console.WriteLine("All data was loaded successfully.");
            else
                Console.WriteLine($"All data was loaded except {numberOfExceptions.ToString()}.");
        }
    }

    class KataCompleted
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

    class KataInfo
    {
        public string description { get; set; }
    }
}
