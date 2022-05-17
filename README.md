# CodewarsGitHubLogger

This program allows you to connect to [Codewars](https://www.codewars.com) by using your credentials
(through the GitHub OAuth sign in option) and extract all the code of each programming language of
every kata you have completed.

**It's discouraged to put your Codewars solutions publicly available because, even though it depends
on every person and it's a responsibility of one self, having your solutions publicly available may
increase the number of dishonest users that copy and paste the solutions to their own advantage. An user
can get sanctioned if the Codewars' staff notice an unusual behaviour. Read the
[Codewars Code of Conduct](https://docs.codewars.com/community/rules/) for more information (especially
the last rule).**

**Although it's not prohibited, please: don't put your Codewars solutions publicly available on GitHub.
Keep your repository private.**

To keep this file simple and short, I only describe here
the most important information, and a summary of other topics. To see all the information, go to the
[repository wiki](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki).

## Why?

A key skill every programmer should have is problem solving (algorithms, paradigms, data structures,
etc.), and Codewars is a very good platform to practice this hability because of its structure, support
of multiple programming languages, user rank system, easily (with moderation) code challenge creation
system and social interaction between users (similar code-challenges, comments, votes, and more).

By having a private repository that contains all the code challenges you have completed, you can keep a
personal registry of all your katas in a more readable way. Moreover, you can allow (temporarily) a recruiter
to see what you've done, so they will know your strenghts and weaknesses, as well as the programming languages
you use with ease. You could do all of this manually, but going to all the pages of solutions of all the
katas you've completed (plus more than one programming language if available) would take you a lot of time
(and you'd have to update it every time you complete a kata).

This repository allows you to copy in your private repository the code of the completed challanges you've
done automatically: you don't have to do anything, just run the program and let it do its job. Moreover,
the files and directories are stored in such a way that it's easy to navigate through them.

## Usage

### Requirements

To use this program you need:

- dotnet core 5.0.
- A Codewars account.
- A GitHub account.

Important things to note: if you want to use the program without modifying it and as-is, **you must have
registered into Codewars using the GitHub OAuth option** and **you must sign in to Codewars before running
the program** so GitHub won't ask you to confirm the sign in with a code. The Firefox driver can't
accomplish this because the code is sent to your email address.

If you registered into Codewars using its own registration system instead of the GitHub OAuth method, you
can change the file to receive and sign in with your Codewars credentials. You can read the subsection
"[Use Codewars credentials instead of GitHub's](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Examples#use-codewars-credentials-instead-of-githubs)" to know how to do this.

### All-in-one method

To use this program you just need an initial configuration. After that, you just have to sit down and watch
everything being pulled automatically. Below is the ordered list of steps you must follow to get all working:

1. Click on the green button "Use this template".
2. Choose a name for your repository, mark the "Private" option and click on "Create repository from template".
3. [Clone](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) or [add remotely](https://docs.github.com/en/get-started/getting-started-with-git/managing-remote-repositories) to pull the new repository locally in your machine.
4. Move to the `CodewarsGitHubLogger` folder.
5. Run the program by writing `dotnet run -- YOUR_CODEWARS_USERNAME YOUR_GITHUB_USERNAME YOUR_GITHUB_PASSWORD` in your command line.
   - Optionally you can append the flag `-i` or `--index` as the last argument to create an index file.
6. Wait for the program to complete (you'll see a message that indicates the completion status).
7. Add all the files to the stage, commit them and push them to your private repository.

(Optional step): add the folder `CodewarsGitHubLogger` to the `.gitignore` file. This is useful if you
don't want to see the folder that contains the program I've created, as it's not a kata by itself.
You can do this without getting into any trouble, because the program is intended to be run locally, so
there's no need in having it visible on GitHub.

For more information about customisation of the repository and the program, read the
["Customisation" section](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Customisation) of the wiki.

### Other methods

Of course, you can just download the entire repository as a `.zip` file and keep only the
[CodewarsGitHubLogger](/CodewarsGitHubLogger) folder. Then, make it so it creates and updates
all the files in the folder you want (the folder that is connected to the private GitHub repository
of your choice). I provide a [simple example of a local run to a different folder](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Examples#create-all-files-in-a-different-directory) in the wiki.

### After completion

And that's it! Keep in mind that it will take some minutes the first time you run it (especially if you
have completed a lot of katas, because the program must loop through all of them). You must run the program
every time you want to update the katas. It takes into consideration repeated files and code refactors.

You can customise some aspects of the program, such as the content of the README.md files,
the name and the directory of the INDEX.md file and other stuff. Head to the ["Customisation" section](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Customisation) in
the wiki of this repository to learn more about what you can do. Also, head to the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Privacy) in
the wiki to learn about the protection of your credentials. I recommend you take a read to the entire
wiki because there is useful and important information for you to know.

If you're facing a problem, go to the ["Failures" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Failures)
of the wiki and look if the error/warning you're getting appears in there. If it doesn't, please open an
issue by choosing a template from the available [issue templates](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/issues/new/choose).

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
on October, 2018).

If you've read above sections, you know you can customise the program to make it work as you want. Keep in
mind that **I offer the program as you see in this repository**. Any changes you made in your private repository
are up to you, and I'm not responsible for them. Be careful on what you change. For more related information,
please read the ["Privacy" page](https://github.com/JoseDeFreitas/CodewarsGitHubLogger/wiki/Privacy) of the
wiki. I encourage you to read the whole wiki, too.
