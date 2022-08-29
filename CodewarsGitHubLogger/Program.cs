/*
Program created by José De Freitas (https://github.com/JoseDeFreitas).
Link to the repository: https://github.com/JoseDeFreitas/CodewarsGitHubLogger.
Licensed under the BSD-2-Clause License.
*/

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
        static Dictionary<string, List<string>> kataCategories = new Dictionary<string, List<string>>() {
            {"reference", new List<string>()}, // Equivalent to the "Fundamentals" category
            {"algorithms", new List<string>()},
            {"bug_fixes", new List<string>()},
            {"refactoring", new List<string>()},
            {"games", new List<string>()} // Equivalent to the "Puzzles" category
        };

        static async Task Main(string[] args)
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new FirefoxDriver("./", options);

            // Define variables for each credential
            string codewarsUsername = "";
            string githubUsername = "";
            string githubPassword = "";

            try
            {
                codewarsUsername = args[0];
                githubUsername = args[1];
                githubPassword = args[2];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("You must pass the credentials.");
                Environment.Exit(1);
            }

            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";
            string mainFolderPath = "../Katas";

            Directory.CreateDirectory(mainFolderPath);

            SignInToCodewars(driver, githubUsername, githubPassword);

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

                    kataCategories[kataInfoObject.category].Add($"- [{kata.name}](./Katas/{kata.slug})");

                    await CreateMainFilesAsync(
                        kataFolderPath, kata.name, kataInfoUrl,
                        kata.id, kata.completedAt, kata.completedLanguages,
                        kataInfoObject.description, kataInfoObject.rank, kataInfoObject.tags
                    );

                    foreach (string language in kata.completedLanguages)
                    {
                        string codeFilePath = Path.Combine(kataFolderPath, $"{kata.slug}.{languagesExtensions[language]}");

                        await CreateCodeFileAsync(driver, codeFilePath, kata.id, language);
                    }
                }
            }

            try
            {
                if (args[3] == "-i" || args[3] == "--index")
                    await CreateIndexFileAsync();
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("'Index' flag was not specified");
            }

            driver.Quit();

            string separatorLine = "-------------------------\n";

            if (numberOfExceptions == 0)
                Console.WriteLine($"{separatorLine}All data was loaded successfully.");
            else
                Console.WriteLine($"{separatorLine}All data was loaded except {numberOfExceptions.ToString()} katas: {string.Join(" - ", idsOfExceptions)}.");
        }

        /// <summary>
        /// Using the credentials provided (GitHub username and passwords) to sign in to
        /// Codewars. It doesn't allow signing in with Codewars creadentials (username and
        /// password) because it's intended to work only if the user registered using the
        /// GitHub OAuth.
        /// </summary>
        /// <param name="driver">The Firefox driver initialised in the Main method.</param>
        /// <param name="githubUsername">The GitHub username of the user.</param>
        /// <param name="githubPassword">The GitHub password of the user.</param>
        /// <exception>When the driver can't connect to the Codewars website.</exception>
        static void SignInToCodewars(IWebDriver driver, string githubUsername, string githubPassword)
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://www.codewars.com/users/sign_in");
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception);
                Environment.Exit(2);
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
            string description, Dictionary<string, object> rank, List<string> tags
        )
        {
            string filePath = Path.Combine(folder, "README.md");
            string content =
            $"# [{name}]({url}{id})\n"
            + $"\n- **Completed at:** {date}\n\n"
            + $"- **Completed languages:** {string.Join(", ", languages)}\n\n"
            + $"- **Tags:** {string.Join(", ", tags)}\n\n"
            + $"- **Rank:** {rank["name"]}\n\n"
            + $"## Description\n\n{description}";

            try
            {
                Directory.CreateDirectory(folder);

                if (File.Exists(filePath))
                {
                    if (String.Compare(await File.ReadAllTextAsync(filePath), content) != 0)
                        await File.WriteAllTextAsync(filePath, content);
                    else
                        return;
                }
                else
                    await File.WriteAllTextAsync(filePath, content);
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
        /// <param name="driver">The Firefox driver initialised in the Main method.</param>
        /// <param name="path">The path of the code file (for each language).</param>
        /// <param name="id">The ID of the kata.</param>
        /// <param name="language">The programming language inside the list of them.</param>
        /// <exception>
        /// When the driver can't connect to the Codewars website or the DOM elements
        /// couldn't be found (due to a problem with Codewars).
        /// </exception>
        static async Task CreateCodeFileAsync(IWebDriver driver, string path, string id, string language)
        {
            IWebElement solutionsList;
            IWebElement solutionItem;
            string solutionCode = "";

            try
            {
                driver.Navigate().GoToUrl($@"https://www.codewars.com/kata/{id}/solutions/{language}/me/newest");

                solutionsList = driver.FindElement(By.Id("solutions_list"));
                solutionItem = solutionsList.FindElement(By.TagName("div"));
                solutionCode = solutionItem.FindElement(By.TagName("pre")).Text;

                if (File.Exists(path))
                {
                    if (String.Compare(await File.ReadAllTextAsync(path), solutionCode) != 0)
                        await File.WriteAllTextAsync(path, solutionCode);
                    else
                        return;
                }
                else
                    await File.WriteAllTextAsync(path, solutionCode);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine(exception);
                return;
            }
            catch (NoSuchElementException)
            {
                return;
            }
            catch (IOException)
            {
                numberOfExceptions++;
                idsOfExceptions.Add(id);
            }
        }

        /// <summary>
        /// If the program was run with the --index flag, this method executes, and
        /// creates a file called "INDEX.md" that lists all the katas based on its
        /// category/discipline, in order to make the navigation through the katas
        /// easier and faster, while being logic.
        /// <summary>
        /// <exception>If the file can't be created.</exception>
        static async Task CreateIndexFileAsync()
        {
            string filePath = "../INDEX.md";
            string content =
            $"# Index of katas by its category/discipline\n\n"
            + $"These are all the code-challenges I've successfully completed. Total completed katas: {{}}"
            + $"\n## Fundamentals\n\n{string.Join("\n", kataCategories["reference"])}"
            + $"\n## Algorithms\n\n{string.Join("\n", kataCategories["algorithms"])}"
            + $"\n## Bug Fixes\n\n{string.Join("\n", kataCategories["bug_fixes"])}"
            + $"\n## Refactoring\n\n{string.Join("\n", kataCategories["refactoring"])}"
            + $"\n## Puzzles\n\n{string.Join("\n", kataCategories["games"])}";

            try
            {
                if (File.Exists(filePath))
                {
                    if (String.Compare(await File.ReadAllTextAsync(filePath), content) != 0)
                        await File.WriteAllTextAsync(filePath, content);
                    else
                        return;
                }
                else
                    await File.WriteAllTextAsync(filePath, content);
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception);
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
        public string category { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }
        public Dictionary<string, object> rank { get; set; }
    }
}
