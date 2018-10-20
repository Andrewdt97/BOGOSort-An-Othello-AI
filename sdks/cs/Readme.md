
# C# SDK for Atomic Games 2018

Written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/), this starter kit includes:

- Cross-platform compatibility via .NET Core
- Networking
- JSON de/serialization
- Command-line parsing
- A unit test project
- Debugger support

## Project directories

- **othello** - console app. Run this!
- **ai** - library to contain the logic.
- **test** - unit test project for ai.

## Using it

Start by installing the latest .NET Core SDK:

https://www.microsoft.com/net/download

You should have the `dotnet` command available:
```
> dotnet --version
2.1.403
```

To run the client, enter the othello directory and `dotnet run -- --help`.

To run the tests, enter the test directory and run `dotnet test`.

## IDEs

I recommend VS Code or JetBrains Rider.

### VS Code

[VS Code](https://code.visualstudio.com/) is Microsoft's free, lightweight, super-extensible editor. 
It's great for editing, and can be coaxed into building, testing, and debugging, though its support is a little scrappy.

Open the project with `code .` from the directory containing the .sln file.

Most of its features can be accessed via a global search with cmd+shift+P (or ctrl+shift+P on Windows/Linux.. I think.).

Some of the interesting things you can do from that menu include:
- Start Debugging
- Start Without Debugging
- Build Project
- Reload Window (for when something stops working)


If things are working then you should get little _Run Test_ / _Debug Test_ "CodeLens" links in test source files.

Read more about VS Code's C# support [here](https://code.visualstudio.com/docs/languages/csharp) and [here](https://github.com/OmniSharp/omnisharp-vscode).


### JetBrains Rider

[Rider](https://www.jetbrains.com/rider/download/) is a commercial product but has a 30-day trial. It will feel familiar if you've used other JetBrains IDEs like IntelliJ, WebStorm, or Android Studio.

Building, testing, and debugging is well-supported.

Open the project by selecting the .sln file.

