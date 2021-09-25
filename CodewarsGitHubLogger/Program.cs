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
        static IWebDriver driver = new FirefoxDriver("./");
        static string githubUsername = Environment.GetEnvironmentVariable("USERNAME_GITHUB");
        static string githubPassword = Environment.GetEnvironmentVariable("PASSWORD_GITHUB");
        static int numberOfExceptions = 0;
        static List<string> idsOfExceptions = new List<string>();
        static Dictionary<string, string> languagesExtensions = new Dictionary<string, string>() {
            {"agda", "agda"}, {"bf", "b"}, {"c", "c"}, {"cmlf", "cmfl"},
            {"clojure", "clj"}, {"cobol", "cob"}, {"coffeescript", "coffee"}, {"commonlisp", "lisp"},
            {"coq", "coq"}, {"cplusplus", "cpp"}, {"crystal", "cr"}, {"csharp", "cs"},
            {"dart", "dart"}, {"elixir", "ex"}, {"elm", "elm"}, {"erlang", "erl"},
            {"factor", "factor"}, {"forth", "fth"}, {"fortran", "f"}, {"fsharp", "fs"},
            {"go", "go"}, {"groovy", "groovy"}, {"haskell", "hs"}, {"haxe", "hx"},
            {"idris", "idr"}, {"java", "java"}, {"javascript", "js"}, {"julia", "jl"},
            {"kotlin", "kt"}, {"lean", "lean"}, {"lua", "lua"}, {"nasm", "nasm"},
            {"nimrod", "nim"}, {"objective", "m"}, {"ocaml", "ml"}, {"pascal", "pas"},
            {"perl", "pl"}, {"php", "php"}, {"powershell", "ps1"}, {"prolog", "pro"},
            {"purescript", "purs"}, {"python", "py"}, {"r", "r"}, {"racket", "rkt"},
            {"ruby", "rb"}, {"rust", "rs"}, {"scala", "scala"}, {"shell", "sh"},
            {"sql", "sql"}, {"swift", "swift"}, {"typescript", "ts"}, {"vb", "vb"},
        };

        static async Task<int> Main(string[] args)
        {
            string codewarsUsername = "JoseDeFreitas"; // Environment.GetEnvironmentVariable("CODEWARS_USERNAME");
            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";
            string mainFolderPath = "../Katas";

            Directory.CreateDirectory(mainFolderPath);

            SigInToCodewars();

            // Response used only to get the total number of pages available
            Stream mainResponseJson = await httpClient.GetStreamAsync(completedKatasUrl);
            KataCompleted mainResponseObject = await JsonSerializer.DeserializeAsync<KataCompleted>(mainResponseJson);
            int numberOfPages = mainResponseObject.totalPages;

            for (int page = 0; page < numberOfPages; page++)
            {
                // Response used to get the information of all the katas in the specified page
                Stream responseStream = await httpClient.GetStreamAsync($"{completedKatasUrl}?page={page}");
                KataCompleted kataObject = await JsonSerializer.DeserializeAsync<KataCompleted>(responseStream);

                foreach (var kata in kataObject.data)
                {
                    // Response used only to get the description of the kata
                    Stream responseKataInfo = await httpClient.GetStreamAsync($"{kataInfoUrl}{kata.id}");
                    KataInfo kataInfoObject = await JsonSerializer.DeserializeAsync<KataInfo>(responseKataInfo);

                    string kataFolderPath = Path.Combine(mainFolderPath, kata.slug);

                    if (!Directory.Exists(kataFolderPath))
                    {
                        await CreateMainFilesAsync(
                            kataFolderPath, kata.name, kataInfoUrl,
                            kata.id, kata.completedAt, kata.completedLanguages,
                            kataInfoObject.description
                        );
                    }

                    foreach (string language in kata.completedLanguages)
                    {
                        string codeFilePath = Path.Combine(kataFolderPath, $"{kata.slug}.{languagesExtensions[language]}");

                        if (!File.Exists(codeFilePath))
                            await CreateCodeFileAsync(codeFilePath, kata.id, language);
                        else
                            continue;
                    }
                }
            }

            driver.Quit();

            if (numberOfExceptions == 0)
                Console.WriteLine("All data was loaded successfully.");
            else
                Console.WriteLine($"All data was loaded except {numberOfExceptions.ToString()} katas: {string.Join(" - ", idsOfExceptions)}.");
            
            return 0;
        }

        /// <summary>
        /// Using the credentials provided (GitHub username and passwords) to sign in to
        /// Codewars. It doesn't allow signing in with Codewars creadentials (username and
        /// password) because it's intended to work only if the user registered using the
        /// GitHub OAuth.
        /// </summary>
        /// <exception>When the driver can't connect to the Codewars website.</exception>
        static void SigInToCodewars()
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://www.codewars.com/users/sign_in");
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception);
                Environment.Exit(1);
            }

            IWebElement siginForm = driver.FindElement(By.Id("new_user"));
            siginForm.FindElement(By.TagName("button")).Click();

            driver.FindElement(By.Id("login_field")).SendKeys(githubUsername);
            driver.FindElement(By.Id("password")).SendKeys(githubPassword);
            driver.FindElement(By.Name("commit")).Click();
        }

        /// <summary>
        /// "Main files" are: the kata folder and the README.md file. The name of the
        /// folder will be the slug of the kata, and the README.md file will contain some
        /// information about the kata (the name, the link to the Codewars page of the kata,
        /// the date of completion, the completed languages, and the description).
        /// </summary>
        /// <param name="folder">The name of the folder (based on the kata slug).</param>
        /// <param name="name">The name of the kata (not the slug).</param>
        /// <param name="url">The url to the kata on the Codewars website.</param>
        /// <param name="id">The ID of the kata.</param>
        /// <param name="date">The date the kata was completed on.</param>
        /// <param name="languages">The programming languages the kata was completed in.</param>
        /// <param name="description">The description of the kata (Markdown-formatted).</param>
        /// <exception>If the folder can't be created.</exception>
        static async Task CreateMainFilesAsync(
            string folder, string name, string url,
            string id, string date, List<string> languages,
            string description
        )
        {
            string[] content =
            {
                $"# [{name}]({url}{id})\n",
                $"**Completed at:** {date}\n",
                $"**Completed languages:** {string.Join(", ", languages)}\n",
                $"## Description\n\n{description}"
            };

            try
            {
                Directory.CreateDirectory(folder);
                await File.WriteAllLinesAsync(Path.Combine(folder, "README.md"), content);
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception);
                numberOfExceptions++;
                idsOfExceptions.Add(id);
            }
        }

        /// <summary>
        /// Pulls the code of the kata from the Codewars website. It supports all the
        /// programming languages the code was completed in. With that code in hand,
        /// copies it to a new file with the proper language extensions and adds it to
        /// the kata folder. One file per programming language completed.
        /// </summary>
        /// <param name="path">The path of the code file (for each language).</param>
        /// <param name="id">The ID of the kata.</param>
        /// <param name="language">The programming language inside the list of them.</param>
        /// <exception>
        /// When the driver can't connect to the Codewars website or the DOM elements
        /// couldn't be found (due to a problem with Codewars).
        /// </exception>
        static async Task CreateCodeFileAsync(string path, string id, string language)
        {
            IWebElement solutionsList;
            IWebElement solutionItem;
            string solutionCode = "";

            string[] code = { solutionCode };

            try
            {
                driver.Navigate().GoToUrl($@"https://www.codewars.com/kata/{id}/solutions/{language}/me/newest");

                solutionsList = driver.FindElement(By.Id("solutions_list"));
                solutionItem = solutionsList.FindElement(By.TagName("li"));
                solutionCode = solutionItem.FindElement(By.TagName("pre")).Text;

                await File.WriteAllLinesAsync(path, code);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception);
                return;
            }
            catch (NoSuchElementException exception)
            {
                Console.WriteLine(exception);
                return;
            }
            catch (IOException)
            {
                numberOfExceptions++;
                idsOfExceptions.Add(id);
            }
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
