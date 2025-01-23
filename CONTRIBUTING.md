# Contributing

## How to

There are two main ways of contributing to CodewarsLogger, the first by
opening an issue and explaining what feature you would like to see being
added to the program, and the second by forking the repository, making
the changes in your copy and then opening a pull request with your
additions. I will revise issues and pull requests to check that everything
works and that it's a great addition, ocassionaly with further comments.

## Code explanation

To better understand some regions of the code (the ones I think are least
clear), I wrote this section.

### Structure of the Codewars API JSON data

The Codewars API is used for getting both the information of the katas the
user completed and the individual information of each kata, each with a
different URL.

An example of the response that targets the user (URL "https://www.codewars.com/api/v1/users/USERNAME/code-challenges/completed",
and with the possible addition of `?page=PAGE` if more than one page):

```json
{
  "totalPages": 2,
  "totalItems": 341,
  "data": [
    {
      "id": "521cd52e790405a74800032c",
      "name": "Semi-Optional",
      "slug": "semi-optional",
      "completedLanguages": [
        "python"
      ],
      "completedAt": "2024-04-16T10:03:36.742Z"
    },
    {
      "id": "644661194e259c035311ada7",
      "name": "Vexing Vanishing Values",
      "slug": "vexing-vanishing-values",
      "completedLanguages": [
        "python"
      ],
      "completedAt": "2024-04-16T09:49:55.761Z"
    },
    {
      "id": "56bcaedfcf6b7f2125001118",
      "name": "Safen User Input Part I - htmlspecialchars",
      "slug": "safen-user-input-part-i-htmlspecialchars",
      "completedLanguages": [
        "python"
      ],
      "completedAt": "2024-04-15T14:28:44.933Z"
    }
  ]
}
```

(Shortened);

An example of the response that targets the kata (URL "https://www.codewars.com/api/v1/code-challenges/ID"):

```json
{
  "id": "644661194e259c035311ada7",
  "name": "Vexing Vanishing Values",
  "slug": "vexing-vanishing-values",
  "category": "bug_fixes",
  "publishedAt": "2023-04-24T11:20:54.412Z",
  "approvedAt": "2023-04-26T05:10:16.567Z",
  "languages": [
    "python"
  ],
  "url": "https://www.codewars.com/kata/644661194e259c035311ada7",
  "rank": {
    "id": -8,
    "name": "8 kyu",
    "color": "white"
  },
  "createdAt": "2023-04-24T10:59:37.481Z",
  "createdBy": {
    "username": "Kacarott",
    "url": "https://www.codewars.com/users/Kacarott"
  },
  "approvedBy": {
    "username": "dfhwze",
    "url": "https://www.codewars.com/users/dfhwze"
  },
  "description": "Confusion, perplexity, a mild headache. These are just a sample of the things you have experienced in the last few minutes while trying to figure out what is going on in your code.\n\nThe task is very simple: accept a list of values, and another value `n`, then return a new list with every value multiplied by `n`. For example, `[1, 2, 3]` and `4` should result in `[4, 8, 12]`.\n\nWhile writing the function, you even added some debugging lines to make sure that you didn't mess anything up, and everything looked good! But for some reason when you run the function it always seems to return an empty list, even though you can clearly see, that the list *should* have the right values in it! Somehow, the values are simply disappearing! Is this a bug in the programming language itself...?",
  "totalAttempts": 5310,
  "totalCompleted": 3152,
  "totalStars": 23,
  "voteScore": 273,
  "tags": [
    "Debugging",
    "Refactoring"
  ],
  "contributorsWanted": false,
  "unresolved": {
    "issues": 0,
    "suggestions": 0
  }
}
```
