# CodewarsGitHubLogger

This program allows you to connect to [Codewars](https://www.codewars.com) by using your credentials
(through the GitHub OAuth sign in option) and extract all the code of each programming language of
every kata you have completed.

To keep this file simple and short, I only describe here
the most important information, and a summary of other topics. To see all the information, go to the
[repository wiki](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki).

## Why?

I developed this small program because GitHub is the biggest portfolio for all kind of programmers
(along with websites and/or other media profiles). A key skill every programmer should have is problem
solving (algorithms, paradigms, data structures, etc.), and Codewars is a very good platform to
practice this hability because of its structure, support of multiple programming languages, user rank
system, easily (with moderation) code challenge creation system and social interaction between users
(similar code-challenges, comments, votes, and more).

By having a repository that contains all the code challenges you have completed, everyone (including
recruiters) can see what you've done, what languages do you use with ease and your strenghts and
weaknesses. You could do this manually, but going to all the pages of solutions of all the katas you've
completed (plus more than one programming language if available) would take you a lot of time (and
you'd have to update it every time you complete a kata).

This repository allows you to copy in your repository the code of the completed challanges you've done
automatically: you don't have to do anything, just run the program and let ir do its job. Moreover, the
files and directories are stored in such a way that it's easy to navigate through them.

## Usage

### Requirements

To use this program you need:

- dotnet core 5.0.
- A Codewars account.
- A GitHub account.

Important things to note: **you must have registered into Codewars using the GitHub OAuth option**
and **you must sign in to Codewars before running the program** so GitHub won't ask you to confirm the
sign in with a code. The Firefox Driver can't accomplish this because the code is sent to your email
direction.

### All-in-one method

To use this program you just need an initial configuration. After that, you just have to sit and watch
everything copying automatically. Below is the ordered list of steps you must follow to get all working:

1. Click on the green button "Use this template".
2. Choose a name for your repository and click on "Create repository from template".
3. Clone or add remotely to the pull the new repository locally in your machine.
4. Move to the `CodewarsGitHubLogger`folder.
5. Run the program doing `dotnet run -- YOUR_CODEWARS_USERNAME YOUR_GITHUB_USERNAME YOUR_GITHUB_PASSWORD`.
   - Optionally you can append the flag `-i` or `--index` as the last argument to create an index file.
6. Wait for the program to complete.
7. Add all the files to the stage, commit them and push them to your repository.

(Optional step): add the folder `CodewarsGitHubLogger` to the `.gitignore` file. This is useful if you
don't want to show the folder that contains the program I've created, as it's not a kata by itself.
You can do this without getting into any trouble, because the program is intended to be run locally, so
there's no need in having it visible in GitHub.

For more information about customisation of the repository and the program, read the
["Customisation" section](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Customisation) of the wiki.

### Other methods

Of course, you can just download the entire repository as a `.zip` file and keep only the
[CodewarsGitHubLogger](/CodewarsGitHubLogger) folder. Then, add the environment variables and make it so
it creates and updates all the files in the folder you want (the folder that is connected to the GitHub
repository of your choice). I provide a [simple example of a local run to a different folder]()
in the wiki.

And that's it! Keep in mind that it will take some time the first time you run it (especially if you
have completed a lot of katas, because the program must loop through all of them). You must run the program
every time you want to update the katas. It takes care of repeated files and code refactors.

You can customize some aspects of the program, such as the content of the README.md files,
the name and the directory of the INDEX.md file and other stuff. Head to the ["Customisation" section](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Customisation) in
the wiki of this repository to learn more about what you can do. Also, head to the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Privacy) in
the wiki to learn about the protection of your credentials. I recommend you take a read to the entire
wiki because there is useful and important information for you to know.

If you're facing a problem, please open an issue by choosing a template from the available
[issue templates](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/issues/new/choose).

## Disclaimer

This repository is under the [BSD-2-Clause License](LICENSE) to follow
[Codewars's Terms of Service](https://www.codewars.com/about/terms-of-service) (that were last updated
on October, 2018). There's no problem in having your solutions to the code-challenges publicly available.

If you've read above sections, you know you can customise the program to make it work as you want. Keep in
mind that **I offer the program as you see in this repository**. Any changes you made in your repository
are up to you, and I'm not responsible for them. Be careful on what you change, especially in the workflow
file. For more related information, please read the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Privacy) of the wiki. I encourage you to read
the whole wiki, too.
