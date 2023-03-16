# CodewarsGitHubLogger
![GitHub all releases](https://img.shields.io/github/downloads/JoseDeFreitas/CodewarsGitHubLogger/total)
![Supported OS versions](https://img.shields.io/badge/for-Windows%2C%20MacOS%2C%20Linux-blue)

This program allows you to connect to [Codewars](https://www.codewars.com) by using your credentials
(through the GitHub OAuth sign in option or through the Codewars sign in form) and extract all the
code of each programming language of every kata you have completed into a folder.

**It's discouraged to put your Codewars solutions publicly available because, even though it depends
on every person and it's a responsibility of one self, having your solutions publicly available may
increase the number of dishonest users that copy and paste the solutions to their own advantage. An user
can get sanctioned if the Codewars' staff notice an unusual behaviour. Read the
[Codewars Code of Conduct](https://docs.codewars.com/community/rules/) for more information (especially
the last rule).**

**Although it's not prohibited, please: don't put your Codewars solutions publicly available on GitHub.**

To see additional information about the program, go to the
[wiki of the repository](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki).

## Why?

A key skill every programmer should have is problem solving (algorithms, paradigms, data structures,
etc.), and Codewars is a very good platform to practice this hability because of its structure, support
of multiple programming languages, user rank system, easily (with moderation) code challenge creation
system and social interaction between users (similar code-challenges, comments, votes, and more).

By having a folder that contains all the code challenges you have completed, you can keep a personal
registry of all your katas in a more readable way. Moreover, you can send the folder to a recruiter so
they can see what you've done, so they will know your strenghts and weaknesses, as well as the
programming languages you use with ease. You could do all of this manually, but going to all the pages
of solutions of all the katas you've completed (plus more than one programming language if available)
would take you a lot of time (and you'd have to update it every time you complete a new kata).

This program allows you to copy in a folder the code of the completed challanges you've done
automatically: you don't have to do anything, just run the program and let it do its job. Moreover,
the files and directories are stored in such a way that it's easy to navigate through them.

## Usage

### Requirements

To use this program you need:

- Firefox web browser installed on your computer.
- GeckoDriver file from Mozilla (which is just an executable of the Mozilla Firefox web browser, so it
can make everything by itself).
- A Codewars account (it doesn't matter if you signed up using the GitHub OAuth or the Codewars form).

**Important thing to note**: if you need to input a GitHub token because you have 2FA authentication,
you'll not be able to use the program. If you use the Codewars login credentials there shouldn't be
any problems.

### Execution

These are the steps that you must follow to run the program:
1. Download the executable file depending on the OS you have (head to the ["Releases" tab](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/releases)).
2. Get the [Mozilla geckodriver](https://github.com/mozilla/geckodriver/releases) from the "Releases" tab in Mozilla's repository (also according to your OS).
3. Run the executable with the geckodriver file in the same directory.
4. Follow the instructions prompted.

You can also clone the repository to have the code in you local machine and run the program from there.
This way it's faster for you to customise whatever you want.

### After completion

And that's it! Keep in mind that it will take some minutes the first time you run it (especially if you
have completed a lot of katas, because the program must loop through all of them). You must run the
program every time you want to update the katas. It takes into consideration repeated files and code
refactors.

If you're facing a problem, go to the ["Failures" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Failures)
of the wiki and look if the error/warning you're getting appears in there. If it doesn't, please
[open an issue](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/issues/new?assignees=&labels=bug&template=bug_report.yaml).

## Screenshots

<details>
   <summary>Main repository page</summary>
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

## Disclaimer

This repository is under the [BSD-2-Clause License](LICENSE) to follow
[Codewars's Terms of Service](https://www.codewars.com/about/terms-of-service) (that were last updated
on April, 2022).

If you've read above sections, you know you can customise the program to make it work as you want. Keep in
mind that **I offer the program as you see it in this repository**. Any changes you made are up to you,
and I'm not responsible for them. Be careful on what you change. For more related information, please read
the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Privacy) of the wiki. I
encourage you to read the whole wiki, too. **Don't forget to read the rules from Codewars.**
