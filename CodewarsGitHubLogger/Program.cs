using System;
using System.IO;
using OpenQA.Selenium;
using System.Net.Http;
using System.Text.Json;
using OpenQA.Selenium.Firefox;
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
            string githubUsername = "USERNAME"; // Environment.GetEnvironmentVariable("GITHUB_USERNAME");
            string githubPassword = "PASSWORD"; // Environment.GetEnvironmentVariable("GITHUB_PASSWORD");
            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";
            string mainFolderPath = "../Katas";
            Dictionary<string, string> languageExtensions = new Dictionary<string, string>();

            Directory.CreateDirectory(mainFolderPath);

            // Start Firefox web browser
            IWebDriver driver = new FirefoxDriver("./");

            try
            {
                driver.Navigate().GoToUrl(@"https://www.codewars.com/users/sign_in");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            IWebElement siginForm = driver.FindElement(By.Id("new_user"));
            siginForm.FindElement(By.TagName("button")).Click();

            driver.FindElement(By.Id("login_field")).SendKeys(githubUsername);
            driver.FindElement(By.Id("password")).SendKeys(githubPassword);
            driver.FindElement(By.Name("commit")).Click();

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

                    // Create folder with the slug name of the kata
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

                    // Create "README.md" file containing information of the kata
                    string[] content =
                    {
                        $"# [{kata.name}]({kataInfoUrl}{kata.id})\n",
                        $"**Completed at:** {kata.completedAt}\n",
                        $"**Completed languages:** {string.Join(", ", kata.completedLanguages)}\n",
                        $"## Description\n\n{kataInfoObject.description}"
                    };

                    await File.WriteAllLinesAsync(Path.Combine(kataFolder, "README.md"), content);

                    foreach (string language in kata.completedLanguages)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl($@"https://www.codewars.com/kata/{kata.id}/solutions/{language}/me/newest");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        IWebElement solutionsList = driver.FindElement(By.Id("solutions_list"));
                        IWebElement solutionItem = solutionsList.FindElement(By.TagName("li"));
                        string solutionCode = solutionItem.FindElement(By.TagName("pre")).Text;
                        
                        // Create file containing the code solution of the kata (based on the programming language)
                        string[] code =
                        {
                            solutionCode
                        };
    
                        await File.WriteAllLinesAsync(Path.Combine(kataFolder, $"{kata.slug}"), code);
                        }
                }
            }

            driver.Quit();

            if (numberOfExceptions == 0)
                Console.WriteLine("All data was loaded successfully.");
            else
                Console.WriteLine($"All data was loaded except {numberOfExceptions.ToString()} katas.");
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
