using System;
using System.IO;

namespace CodewarsGitHubLogger
{
    class Program
    {
        static string folderPath = "../Katas/";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
