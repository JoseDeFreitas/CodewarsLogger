# CodewarsLogger

![GitHub all releases](https://img.shields.io/github/downloads/JoseDeFreitas/CodewarsLogger/total)
![Supported OS versions](https://img.shields.io/badge/for-Windows%2C%20MacOS%2C%20Linux-blue)
![Architecture of 64 bits](https://img.shields.io/badge/architecture-64--bit-purple)

> Also check [Codewars Activity Chart](https://github.com/JoseDeFreitas/codewars-activity-chart)!

This program allows you to connect to [Codewars](https://www.codewars.com) by using your Codewars
credentials and extract all the code of each programming language of every kata you have completed
into a folder.

It's discouraged to put your Codewars solutions publicly available because, even though it depends
on every person, this may increase the number of dishonest users that copy and paste the solutions
to their own advantage. An user can get sanctioned if the Codewars' staff notice an unusual behaviour.
Read the [Codewars Code of Conduct](https://docs.codewars.com/community/rules/)
for more information (especially the last rule).

**Although it's not prohibited, please: don't put your Codewars solutions publicly available on GitHub.**

## Screenshots

<details>
   <summary>README file (index)</summary>
   <img src="https://user-images.githubusercontent.com/37962411/168847084-a6d12825-881e-414c-8ae3-0c7655c7a63a.png">
</details>

<details>
   <summary>Folders of the katas</summary>
   <img src="https://user-images.githubusercontent.com/37962411/168847279-53692969-6a12-4b2e-8ed6-85b929d49beb.png">
</details>

<details>
   <summary>Inside a kata folder</summary>
   <img src="https://user-images.githubusercontent.com/37962411/168847303-9f71d056-5888-489d-804d-12c0b330ad91.png">
</details>

<details>
   <summary>Solution of a kata</summary>
   <img src="https://user-images.githubusercontent.com/37962411/168847325-97818ed9-da90-4dda-bb41-cbaa6b04cc2c.png">
</details>

## Why?

A key skill every programmer should have is problem solving (algorithms, paradigms, data structures,
etc.), and Codewars is a very good platform to practice this hability because of its structure: support
of multiple programming languages, user rank system, easily (with moderation) code challenge creation
system and social interaction between users (similar code-challenges, comments, votes, and more).

By having a folder that contains all the code challenges you have completed, you can keep a personal
registry of all your katas in a more readable way. Moreover, you can send the folder to a recruiter so
they can see what you've done, get to know your strenghts and weaknesses, and see what are the
programming languages you use with ease. You could do all of this manually, but going through all the pages
of solutions of all the katas you've completed (plus more than one programming language, depending on the case)
would take you a lot of time, and you'd have to update it every time you complete a new kata.

This program allows you to automatically copy into a folder the code of the completed challanges you've
done: you just have to run the program and let it do its job. The files and directories are stored in
such a way that it's easy to navigate through them.

## Usage

### Requirements

To use this program you need:

- .NET runtime installed on your computer.
- Firefox web browser installed on your computer.
- Geckodriver file from Mozilla.

**Important thing to note**: If you registered into Codewars using the GitHub OAuth option, you must
create a Codewars password. You can do this easily by clicking on the text "Forgot your password?"
in the [Codewars sign in page](https://www.codewars.com/users/sign_in).

### Execution

These are the steps that you must follow to run the program:
1. Download the program folder depending on the OS you use (head to the ["Releases" tab](https://github.com/JoseDeFreitas/CodewarsLogger/releases)).
2. Download the [.NET runtime](https://dotnet.microsoft.com/en-us/download) from the official Microsoft website
(currently, the program is targeted to the version 8.0).
3. Download the [Mozilla geckodriver](https://github.com/mozilla/geckodriver/releases) from the "Releases" tab in Mozilla's repository.
4. Check the "config.txt" file to see if the Firefox binary location is actually in that directory
(change it if it's not the case; write it just after `DIRECTORY=`).
5. With the "geckodriver.exe" file in the program's directory, run the executable.
6. Follow the instructions prompted. If you're running it for the first time, choose "n", as there is
not any last kata saved yet.

You can also clone the repository to have the code in you local machine and run the program from there.
This way it's faster for you to customise whatever you want. For this, use
`git clone https://github.com/JoseDeFreitas/CodewarsLogger.git`.
You need to have [.NET](https://dotnet.microsoft.com/en-us/download) and Firefox installed. If you do, go to the
"CodewarsLogger" folder (where the "Program.cs" lies) and type `dotnet run` in the console.

### After completion

And that's it! Keep in mind that it will take time depending on how many katas you've completed, since it
must loop through all of them. When the 429 HTTP error ("Too Many Requests") gets thrown, the program
automatically stops the execution and waits 2 minutes before continuing with the loop; this happens
after approximately 250 API calls.

The program let's you choose whether to run it through all of the katas or just until the last saved one.
Every time you run the program, the slug of the last completed kata is saved in the "config.txt" file
next to the `LAST_KATA=` text. Choosing to run the program until encountering this kata is useful if
you know you haven't trained any previous kata and just want to update the folder by adding the new
katas you have completed, massively decreasing the time of execution. If you refactored the code of a
kata you completed in the past, you would have to not choose that option and let it loop through all of
the katas to copy the new code and replace it. I haven't found a better solution for this issue.

For some reason, the API endpoint of Codewars that lists the "completed" katas includes also katas that
you have trained but that aren't literally "completed"; that is, katas that have some solution that didn't
pass all the tests. You complete a kata when you click the blue button "Attempt" and then the green button
"Submit". If you get a list of exceptions (katas which code couldn't be found) is because of this, since
Codewars shows a text saying "No solutions" instead of the list of solutions that the program looks for.

For a clearer view of how the program goes through all the katas, **you can comment out the [line 120](https://github.com/JoseDeFreitas/CodewarsLogger/blob/80f372c874595b06d8591ff084665fae77f96e03/CodewarsLogger/Program.cs#L120)**
(`options.AddArgument("--headless");`) by adding `//` before it.

If you're facing a problem, go to the ["Errors" page](https://github.com/JoseDeFreitas/CodewarsLogger/wiki/Errors)
of the wiki and look if the error/warning you're getting appears in there. If it doesn't, please
[open an issue](https://github.com/JoseDeFreitas/CodewarsLogger/issues/new?assignees=&labels=bug&template=bug_report.yaml).

## Disclaimer

This repository is under the [BSD-2-Clause License](LICENSE) to follow
[Codewars's Terms of Service](https://www.codewars.com/about/terms-of-service) (that were last updated
on April, 2022).

If you've read above sections, you know you can customise the program to make it work as you want. Keep in
mind that **I offer the program as you see it in this repository**. Any changes you made are up to you,
and I'm not responsible for them. Be careful on what you change. For more related information, please read
the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsLogger/wiki/Privacy) of the wiki. I
encourage you to read the whole wiki, too. **Don't forget to read the rules from Codewars.**
