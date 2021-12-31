# fs-bossa-nol-runner

The source code of the [BossaNolRunner NuGet package](https://www.nuget.org/packages/BossaNolRunner/) which alows user to run [NOL 3](http://bossa.pl/oferta/internet/pomoc/nol/) application automatically using **Chrome** browser from .NET code.

Project uses:

* [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/)
* [F#](https://fsharp.org)
* [Playwright](https://playwright.dev/)
* [Argu](http://fsprojects.github.io/Argu/)

Running NOL3 is done (automatically) in 3 simple steps:

* opening Chrome browser
* login to [bossa.pl account](https://online.bossa.pl/bossaapp/login) using credentials provided as an application parameters or stored in environment variable
* starting NOL 3

## Prerequisites (Playwright and browser installation)

In order to make an application working it is necessary to install [Playwright CLI Tool](https://www.nuget.org/packages/Microsoft.Playwright.CLI/):

```ps
dotnet tool install --global Microsoft.Playwright.CLI --version 1.2.2
```

Then using Playwright CLI it is necessary to install browser(s):

```ps
playwright install
```

## Credentials

User name and password to Bossa.pl account can be provided:

* as an application parameter: **--credentials [userName] [password]**
* as an **bossaCredentials** environment variable separated by semicolon:
  
  ![EnvironmentVariables](docs/assets/EnvironmentVariables.png)

  ![BossaCredentials](docs/assets/BossaCredentials.png)

## Usage

Usage examples from F# or C# are in [Sample](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples) folder:

* **[Sample/NolRunnerAppFs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppFs)** folder contains F# application
* **[Sample/NolRunnerAppCs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppCs)** folder contains C# application

After compilation from within corresponding folder (NolRUnnerAppFs or NoRunnerAppCs):
  
```ps
dotnet build
```

then the application can be executed with parameters:

```ps
dotnet run --credentials [username] [password]
```

e.g.:

![Parameters](docs/assets/FsBossaNolRunnerExe.png)
  
If credentials parameters are not provided then ***user name*** and ***password*** will be taken from environment variable **'bossaCredentials'**. If there are no credentials provided as an application parameter nor stored in environment variable then the application will not run (neither browser nor NOL3).

The package was tested using:

* **Windows 10** 21H2 19044.1415
* **Chrome**  96.0.4664.110
* **NOL 3** 3.1.15.244.I.7
* [**VS2022 17.0.4**](https://www.visualstudio.com/pl/downloads) (with F# Desktop Components)

*The package will not be actively maintained.*
