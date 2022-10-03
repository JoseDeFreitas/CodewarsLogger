/*
Program created by José de Freitas (https://github.com/JoseDeFreitas).
Link to the repository: https://github.com/JoseDeFreitas/CodewarsGitHubLogger.
Licensed under the BSD-2-Clause License.
*/

using System;
using System.IO;
using System.Linq;
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
        static HttpClient Client = new HttpClient();
        static int CompletedKatasCount = 0;
        static List<string> IdsOfExceptions = new List<string>();
        static Dictionary<string, string> LanguagesExtensions = new Dictionary<string, string>() {
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
        static Dictionary<string, List<string>> KataCategories = new Dictionary<string, List<string>>() {
            {"reference", new List<string>()}, // Equivalent to the "Fundamentals" category
            {"algorithms", new List<string>()},
            {"bug_fixes", new List<string>()},
            {"refactoring", new List<string>()},
            {"games", new List<string>()} // Equivalent to the "Puzzles" category
        };

        static async Task Main(string[] args)
        {
            List<string> credentials = ReadUserCredentials();

            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new FirefoxDriver("./", options);

            // Define variables for each credential
            string loginMethod = "";
            string codewarsUsername = "";
            string usernameOrEmail = "";
            string password = "";

            try
            {
                loginMethod = credentials[1];
                codewarsUsername = credentials[0];
                usernameOrEmail = credentials[2];
                password = credentials[3];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("You must pass all the credentials.");
                Environment.Exit(1);
            }

            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";
            string mainFolderPath = "./Katas";

            Directory.CreateDirectory(mainFolderPath);

            SignInToCodewars(driver, loginMethod, usernameOrEmail, password);

            // Response used only to get the total number of pages available
            Stream mainResponseJson = await Client.GetStreamAsync(completedKatasUrl);
            KataCompleted mainResponseObject = await JsonSerializer.DeserializeAsync<KataCompleted>(mainResponseJson);
            int numberOfPages = mainResponseObject.totalPages;

            for (int page = 0; page < numberOfPages; page++)
            {
                // Response used to get the information of all the katas in the specified page
                Stream responseStream = await Client.GetStreamAsync($"{completedKatasUrl}?page={page}");
                KataCompleted kataObject = await JsonSerializer.DeserializeAsync<KataCompleted>(responseStream);

                foreach (var kata in kataObject.data)
                {
                    // Response used only to get the description of the kata
                    Stream responseKataInfo = await Client.GetStreamAsync($"{kataInfoUrl}{kata.id}");
                    KataInfo kataInfoObject = await JsonSerializer.DeserializeAsync<KataInfo>(responseKataInfo);
                    string kataFolderPath = Path.Combine(mainFolderPath, kata.slug);

                    KataCategories[kataInfoObject.category].Add($"- [{kata.name}](./Katas/{kata.slug})");

                    await CreateMainFileAsync(
                        kataFolderPath, kata.name, kata.id,
                        kata.completedAt, kata.completedLanguages,
                        kataInfoObject.description, kataInfoObject.rank, kataInfoObject.tags
                    );

                    CompletedKatasCount++;

                    foreach (string language in kata.completedLanguages)
                    {
                        string codeFilePath = Path.Combine(kataFolderPath, $"{kata.slug}.{LanguagesExtensions[language]}");
                        await CreateCodeFileAsync(driver, codeFilePath, kata.id, language);
                    }
                }
            }

            await CreateIndexFileAsync();

            driver.Quit();

            IdsOfExceptions = IdsOfExceptions.Distinct().ToList();
            if (IdsOfExceptions.Count == 0)
                Console.WriteLine("\nAll data was loaded successfully.");
            else
            {
                Console.WriteLine(
                    $"\nAll data was loaded successfully except {IdsOfExceptions.Count} katas: {string.Join("\n", IdsOfExceptions)}."
                );
            }

            Console.Write("Press any key to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Serves as the main view of the console application by asking the user for the
        /// necessary credentials (Codewars' or GitHub's) and saving all the information
        /// and sending it to the main method.
        /// </summary>
        /// <returns>
        /// A list of 4 strings: the Codewars username, the email/GitHub username, the
        /// Codewars password/GitHub password and whether to create an index file or not
        /// ("y" or "n").
        /// </returns>
        static List<string> ReadUserCredentials()
        {
            List<string> credentials = new List<string>();
            bool loopFlag = true;

            Console.WriteLine("CodewarsGitHubLogger, v1.1.3. Source code: https://github.com/JoseDeFreitas/CodewarsGitHubLogger");
            Console.Write("Enter your Codewars username: ");
            credentials.Add(Console.ReadLine());
            Console.Write("Press \"g\" to log using GitHub or \"c\" to log using Codewars: ");

            string loginMethod = Console.ReadLine();
            while (loopFlag)
            {
                if (loginMethod == "g")
                {
                    credentials.Add(loginMethod);
                    Console.Write("Enter your GitHub username: ");
                    credentials.Add(Console.ReadLine());
                    Console.Write("Enter your GitHub password: ");
                    credentials.Add(Console.ReadLine());

                    loopFlag = false;
                }
                else if (loginMethod == "c")
                {
                    credentials.Add(loginMethod);
                    Console.Write("Enter your email: ");
                    credentials.Add(Console.ReadLine());
                    Console.Write("Enter your Codewars password: ");
                    credentials.Add(Console.ReadLine());

                    loopFlag = false;
                }
                else
                {
                    Console.WriteLine("Only \"g\" and \"c\" are valid options.");
                }
            }

            return credentials;
        }

        /// <summary>
        /// Using the credentials provided (GitHub username and password or email and Codewars
        /// password) to sign in to Codewars.
        /// </summary>
        /// <param name="driver">The Firefox driver initialised in the Main method.</param>
        /// <param name="loginMethod">The Firefox driver initialised in the Main method.</param>
        /// <param name="usernameOrEmail">The GitHub username or the email of the user.</param>
        /// <param name="password">The GitHub password or the Codewars password of the user.</param>
        /// <exception>When the driver can't connect to the Codewars website.</exception>
        static void SignInToCodewars(
            IWebDriver driver, string loginMethod, string usernameOrEmail,
            string password
        )
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://www.codewars.com/users/sign_in");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("The driver took too much time.");
                Environment.Exit(1);
            }

            if (loginMethod == "g")
            {
                try
                {
                    IWebElement siginForm = driver.FindElement(By.Id("new_user"));
                    siginForm.FindElement(By.TagName("button")).Click();

                    driver.FindElement(By.Id("login_field")).SendKeys(usernameOrEmail);
                    driver.FindElement(By.Id("password")).SendKeys(password);
                    driver.FindElement(By.Name("commit")).Click();
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("The element was not found on the page.");
                    Environment.Exit(1);
                }
            }
            else if (loginMethod == "c")
            {
                try
                {
                    IWebElement siginForm = driver.FindElement(By.Id("new_user"));

                    driver.FindElement(By.Id("user_email")).SendKeys(usernameOrEmail);
                    driver.FindElement(By.Id("user_password")).SendKeys(password);
                    siginForm.FindElement(By.ClassName("is-red")).Click();
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("The element was not found on the page.");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// "Main files" are: the kata folder and the README.md file. The name of the
        /// folder will be the slug of the kata, and the README.md file will contain some
        /// information about the kata (the name, the link to the Codewars page of the kata,
        /// the date of completion, the completed languages, and the description).
        /// </summary>
        /// <param name="folder">The name of the folder (based on the kata slug).</param>
        /// <param name="name">The name of the kata (not the slug).</param>
        /// <param name="id">The ID of the kata.</param>
        /// <param name="date">The date the kata was completed on.</param>
        /// <param name="languages">The programming languages the kata was completed in.</param>
        /// <param name="description">The description of the kata (Markdown-formatted).</param>
        /// <exception>If the folder can't be created.</exception>
        static async Task CreateMainFileAsync(
            string folder, string name, string id,
            string date, List<string> languages, string description,
            Dictionary<string, object> rank, List<string> tags
        )
        {
            string filePath = Path.Combine(folder, "README.md");
            string content =
            $"# [{name}](https://www.codewars.com/kata/{id})\n\n"
            + $"- **Completed at:** {date}\n\n"
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
            catch (IOException)
            {
                Console.WriteLine("There was a problem while creating the main file.");
                IdsOfExceptions.Add(id);
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
            catch (TimeoutException)
            {
                Console.WriteLine("The driver took too much time.");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("The element was not found on the page.");
            }
            catch (IOException)
            {
                Console.WriteLine("There was a problem while creating the code file.");
                IdsOfExceptions.Add(id);
            }
        }

        /// <summary>
        /// Creates a "README.md" that lists all the katas based on its category/
        /// discipline, in order to make the navigation through the katas easier
        /// and faster.
        /// <summary>
        /// <exception>If the file can't be created.</exception>
        static async Task CreateIndexFileAsync()
        {
            string filePath = "./README.md";
            string content =
            $"# Index of katas by its category/discipline\n\n"
            + $"These are the {CompletedKatasCount} code challenges I have completed:"
            + $"\n## Fundamentals\n\n{string.Join("\n", KataCategories["reference"])}"
            + $"\n## Algorithms\n\n{string.Join("\n", KataCategories["algorithms"])}"
            + $"\n## Bug Fixes\n\n{string.Join("\n", KataCategories["bug_fixes"])}"
            + $"\n## Refactoring\n\n{string.Join("\n", KataCategories["refactoring"])}"
            + $"\n## Puzzles\n\n{string.Join("\n", KataCategories["games"])}";

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
            catch (IOException)
            {
                Console.WriteLine("There was a problem while creating the index file.");
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
