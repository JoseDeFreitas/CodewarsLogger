/*
Program created by José de Freitas (https://github.com/JoseDeFreitas).
Link to the repository: https://github.com/JoseDeFreitas/CodewarsLogger.
Licensed under the BSD-2-Clause License.
*/

using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using System.Collections.Generic;

namespace CodewarsLogger
{
    class Program
    {
        private static readonly HttpClient Client = new();
        private static int CompletedKatasCount = 0;
        private static List<string> IdsOfExceptions = new();
        private static readonly Dictionary<string, string> LanguagesExtensions = new() {
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
        private static readonly Dictionary<string, List<string>> KataCategories = new() {
            {"reference", new List<string>()}, // Equivalent to the "Fundamentals" category
            {"algorithms", new List<string>()},
            {"bug_fixes", new List<string>()},
            {"refactoring", new List<string>()},
            {"games", new List<string>()} // Equivalent to the "Puzzles" category
        };
        private static IWebDriver Driver;

        static async Task Main()
        {
            Initialise();
            (string codewarsUsername, string email, string codewarsPassword) = ReadUserCredentials();

            string mainFolderPath = Path.Combine(Environment.CurrentDirectory, "Katas");
            SignInToCodewars(Driver, email, codewarsPassword);
            Directory.CreateDirectory(mainFolderPath);
            await NavigateWebsite(codewarsUsername, mainFolderPath);

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
        /// Checks for all the necessary files ("geckodriver.exe" and "firefox_location.txt)
        /// to be present and creates the Driver object.
        /// </summary>
        static void Initialise()
        {
            string firefoxDirectory = "";
            try
            {
                using (var fileRead = new StreamReader("firefox_location.txt"))
                {
                    firefoxDirectory = fileRead.ReadToEnd();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"\"firefox_location.txt\" was not found.\n{e.Message}");
            }

            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "geckodriver.exe")))
            {
                throw new FileNotFoundException(
                    "\"geckodriver.exe\" file must be in the program's directory."
                );
            }

            FirefoxOptions options = new()
            {
                BrowserExecutableLocation = firefoxDirectory
            };
            options.AddArgument("--headless");

            try
            {
                Driver = new FirefoxDriver(Environment.CurrentDirectory, options);
            }
            catch (WebDriverArgumentException e)
            {
                Console.WriteLine($"The Firefox executable couldn't be found.\n{e.Message}");
            }
        }

        /// <summary>
        /// Serves as the main view of the console application by asking the user for the
        /// necessary Codewars credentials and saving all the information and sending it
        /// to the main method.
        /// </summary>
        /// <returns>
        /// 3 strings: the Codewars username, the email, and the Codewars password.
        /// </returns>
        static (string, string, string) ReadUserCredentials()
        {
            Console.WriteLine("CodewarsLogger, v1.3.1. Source code: https://github.com/JoseDeFreitas/CodewarsLogger");

            Console.Write("Enter your Codewars username: ");
            string codewarsUsername = Console.ReadLine();
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            Console.Write("Enter your Codewars password: ");
            string codewarsPassword = Console.ReadLine();

            return (codewarsUsername, email, codewarsPassword);
        }

        /// <summary>
        /// Use the credentials provided to sign in to Codewars.
        /// </summary>
        /// <param name="driver">The Firefox driver initialised in the Main method.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="codewarsPassword">The Codewars password of the user.</param>
        /// <exception>When the driver can't connect to the Codewars website.</exception>
        static void SignInToCodewars(
            IWebDriver driver, string email, string codewarsPassword
        )
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://www.codewars.com/users/sign_in");
            }
            catch (TimeoutException e)
            {
                Console.WriteLine($"\rThe driver took too much time.\n{e.Message}");
            }

            try
            {
                IWebElement siginForm = driver.FindElement(By.Id("new_user"));
                driver.FindElement(By.Id("user_email")).SendKeys(email);
                driver.FindElement(By.Id("user_password")).SendKeys(codewarsPassword);
                siginForm.FindElement(By.ClassName("is-red")).Click();
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine($"\rA web element was not found on the page (sign-in step).\n{e.Message}");
            }
        }

        /// <summary>
        /// The primary method of the program. It cicles through all of the katas the user
        /// has completed and creates the corresponding folders and files. Also, it keeps
        /// track of other information, such as the global data structure that saves the
        /// data of the categories. It creates and updates the progress bar.
        /// </summary>
        /// <param name="codewarsUsername">The Codewars name of the user.</param>
        /// <param name="mainFolderPath">.The "/Katas" directory string.</param>
        static async Task NavigateWebsite(string codewarsUsername, string mainFolderPath)
        {
            string completedKatasUrl = $"https://www.codewars.com/api/v1/users/{codewarsUsername}/code-challenges/completed";
            string kataInfoUrl = "https://www.codewars.com/api/v1/code-challenges/";

            // Response used only to get the total number of pages available
            Stream mainResponseJson = await Client.GetStreamAsync(completedKatasUrl);
            KataCompleted mainResponseObject = await JsonSerializer.DeserializeAsync<KataCompleted>(mainResponseJson);
            int numberOfPages = mainResponseObject.totalPages;
            int numberOfKatas = mainResponseObject.totalItems;

            Console.WriteLine($"You have completed a total of {numberOfKatas} katas.");

            int currentKataNumber = 1;

            for (int page = 0; page < numberOfPages; page++)
            {
                // Response used to get the information of all the katas in the specified page
                Stream responseStream = await Client.GetStreamAsync($"{completedKatasUrl}?page={page}");
                KataCompleted kataObject = await JsonSerializer.DeserializeAsync<KataCompleted>(responseStream);

                foreach (var kata in kataObject.data)
                {
                    string pureKataName = string.Join("", kata.slug.Split('/', '\\', ':', '<', '>', '"', '|', '*', '?'));

                    // Response used only to get the description of the kata
                    Stream responseKataInfo = await Client.GetStreamAsync($"{kataInfoUrl}{kata.id}");
                    KataInfo kataInfoObject = await JsonSerializer.DeserializeAsync<KataInfo>(responseKataInfo);
                    string kataFolderPath = Path.Combine(mainFolderPath, pureKataName);

                    KataCategories[kataInfoObject.category].Add($"- [{kata.name}](./Katas/{pureKataName})");

                    await CreateMainFileAsync(
                        kataFolderPath, kata.name, kata.id,
                        kata.completedAt, kata.completedLanguages,
                        kataInfoObject.description, kataInfoObject.rank, kataInfoObject.tags
                    );

                    CompletedKatasCount++;

                    foreach (string language in kata.completedLanguages)
                    {
                        string codeFilePath = Path.Combine(kataFolderPath, $"{pureKataName}.{LanguagesExtensions[language]}");
                        await CreateCodeFileAsync(Driver, codeFilePath, kata.id, language);
                    }

                    // Create and update the progress bar based on the amount of katas
                    double progressPercentage = currentKataNumber / (double)numberOfKatas * 100;
                    Console.Write($"\rProgress: {(int)progressPercentage}%");

                    currentKataNumber++;
                }
            }

            await CreateIndexFileAsync();

            Driver.Quit();
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
        /// <param name="rank">The rank of the kata.</param>
        /// <param name="tags">The tags of the kata.</param>
        /// <exception>If the folder can't be created.</exception>
        static async Task CreateMainFileAsync(
            string folder, string name, string id,
            string date, List<string> languages, string description,
            Dictionary<string, object> rank, List<string> tags
        )
        {
            string path = Path.Combine(folder, "README.md");
            string content =
            $"# [{name}](https://www.codewars.com/kata/{id})\n\n"
            + $"- **Completed at:** {date.Substring(0, 10)}\n\n"
            + $"- **Completed languages:** {string.Join(", ", languages)}\n\n"
            + $"- **Tags:** {string.Join(", ", tags)}\n\n"
            + $"- **Rank:** {rank["name"]}\n\n"
            + $"## Description\n\n{description}";

            try
            {
                Directory.CreateDirectory(folder);
                await CreateFile(path, content);
            }
            catch (IOException e)
            {
                Console.WriteLine($"\rThere was a problem while creating the main file.\n{e.Message}");
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
            string content;

            try
            {
                driver.Navigate().GoToUrl($@"https://www.codewars.com/kata/{id}/solutions/{language}/me/newest");

                solutionsList = driver.FindElement(By.Id("solutions_list"));
                solutionItem = solutionsList.FindElement(By.TagName("div"));
                content = solutionItem.FindElement(By.TagName("pre")).Text;

                await CreateFile(path, content);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine($"\rThe driver took too much time.\n{e.Message}");
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine($"\rA web element was not found on the page (create code file step).\n{e.Message}");
                IdsOfExceptions.Add(id);
            }
            catch (IOException e)
            {
                Console.WriteLine($"\rThere was a problem while creating the code file.\n{e.Message}");
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
            string path = Path.Combine(Environment.CurrentDirectory, "README.md");
            string content =
            $"# Index of katas by its category/discipline\n\n"
            + $"These are the {CompletedKatasCount} code challenges I have completed:"
            + $"\n\n## Fundamentals\n\n{string.Join("\n", KataCategories["reference"])}"
            + $"\n\n## Algorithms\n\n{string.Join("\n", KataCategories["algorithms"])}"
            + $"\n\n## Bug Fixes\n\n{string.Join("\n", KataCategories["bug_fixes"])}"
            + $"\n\n## Refactoring\n\n{string.Join("\n", KataCategories["refactoring"])}"
            + $"\n\n## Puzzles\n\n{string.Join("\n", KataCategories["games"])}";

            try
            {
                await CreateFile(path, content);
            }
            catch (IOException e)
            {
                Console.WriteLine($"\rThere was a problem while creating the README file.\n{e.Message}");
            }
        }

        /// <summary>
        /// Checks if the file exists, handles the relative creation of files
        /// and compares the content of old and new files.
        /// <summary>
        static async Task CreateFile(string filePath, string fileContent)
        {
            if (File.Exists(filePath))
            {
                if (string.Compare(await File.ReadAllTextAsync(filePath), fileContent) != 0)
                    await File.WriteAllTextAsync(filePath, fileContent);
                else
                    return;
            }
            else
                await File.WriteAllTextAsync(filePath, fileContent);
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
