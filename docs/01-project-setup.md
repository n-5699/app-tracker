# Part 1 — Project setup

## Goal
Install the .NET 10 SDK and create an empty ASP.NET Core **Razor Pages** web app that runs in the browser. Every later part builds on this project.

## Prerequisites
- A computer running Windows, macOS, or Linux
- An editor: **Visual Studio 2022+** (Windows) or **Visual Studio Code** with the **C# Dev Kit** extension
- **Git**

## Steps

### 1. Install the .NET 10 SDK
- **Windows:** download the installer from <https://dotnet.microsoft.com/download/dotnet/10.0>, or run `winget install Microsoft.DotNet.SDK.10`
- **macOS / Linux** (no admin rights needed):
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  bash dotnet-install.sh --channel 10.0 --install-dir ~/.dotnet
  ```
  Then add it to your shell profile (`~/.zshrc` or `~/.bashrc`):
  ```bash
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
  ```

Check it worked:
```bash
dotnet --version   # should print 10.0.xxx
```

### 2. Create the solution and web project
```bash
mkdir app-registry && cd app-registry

dotnet new sln -n AppRegistry --format sln                              # the solution file that groups projects
dotnet new webapp -n AppRegistry.Web -o src/AppRegistry.Web -f net10.0  # Razor Pages web app
dotnet sln AppRegistry.sln add src/AppRegistry.Web/AppRegistry.Web.csproj
dotnet new gitignore                                                    # keeps bin/, obj/, secrets out of git
```

### 3. Build and run
```bash
dotnet build
cd src/AppRegistry.Web
dotnet run
```
Open the `http://localhost:xxxx` address printed in the terminal. Press `Ctrl+C` to stop.

In **Visual Studio**, open `AppRegistry.sln` and press **F5** instead.

### 4. Save to Git
```bash
git init
git add .
git commit -m "Part 1: project setup"
```

## What was created

```
app-registry/
├── AppRegistry.sln                 ← solution: opens everything in Visual Studio
├── .gitignore
└── src/AppRegistry.Web/
    ├── AppRegistry.Web.csproj      ← project file: target framework (net10.0), packages
    ├── Program.cs                  ← app startup: registers services, sets up the request pipeline
    ├── appsettings.json            ← configuration (connection strings come later)
    ├── Properties/launchSettings.json ← local URLs/ports for `dotnet run`
    ├── Pages/                      ← one .cshtml (HTML + Razor) + .cshtml.cs (C# code) per page
    │   ├── Index.cshtml            ← home page
    │   └── Shared/_Layout.cshtml   ← shared header, nav bar and footer around every page
    └── wwwroot/                    ← static files: CSS, JavaScript, Bootstrap
```

## How to verify
- [ ] `dotnet --version` prints `10.0.x`
- [ ] `dotnet build` ends with **0 Error(s)**
- [ ] `dotnet run` and the browser shows the **"Welcome"** page

## What you learned / why
- **Solution vs. project:** the `.sln` groups projects. There is only one project now, but a test project could be added later.
- **Razor Pages:** each page is a pair of files. `Index.cshtml` holds the markup and `Index.cshtml.cs` holds its C# "page model" (the code that loads and saves data). This is the simplest Microsoft pattern for form-and-list apps like this one.
- **Program.cs** is where later parts register the database (Entity Framework Core).
- **.gitignore** keeps build output and local secrets out of the shared repository.

**Next:** [Part 2 — Data model + database](02-data-model-and-database.md)
