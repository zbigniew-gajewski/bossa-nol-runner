# fs-bossa-nol-runner

The source code of the BossaNolRunner NuGet package which alows user automatically run **[NOL 3](http://bossa.pl/oferta/internet/pomoc/nol/)** application from .NET using **Chrome**, **FireFox** or **Internet Explorer** browser.

Project is built with:

* [.NET Core 2.2](https://dotnet.github.io/)
* [F#](https://fsharp.org)
* [Canopy](https://lefthandedgoat.github.io/canopy/) (F# wrapper for [Selenium](https://www.seleniumhq.org/))
* [Argu](http://fsprojects.github.io/Argu/)

Running NOL3 is done in 3 simple steps:

* opening a browser (Chrome, FireFox or Internet Explorer)
* login to [bossa.pl account](https://www.bossa.pl) using credentials provided as an application parameters or stored in environment variable
* starting NOL 3

## Browser Drivers

The application which uses this package also needs browser driver (for Chrome, FireFox or Internet Explorer accordingly). Drivers can be:

* downloaded from [Selenium download page](https://www.seleniumhq.org/download/) and copied into execution folder of the final application
* added as a .Net Core packages (see [Samples](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples))

In order to make Selenium driver working correctly, **Internet Explorer** requires all ***internet zones*** parameter (*'Internet'*, *'Local intranet'*, *'Trusted sites'*, *'Restricted sites'*) set with identical ***'Enable Protected Mode...'*** settings (checked prefered):

![IeSettings](docs/assets/BrowserSettings.png)

## Browser parameter

The package can run Chrome, FireFox or Internet Explorer browser by providing **--browser** parameter:

* **--browser chrome** for running Chrome
* **--browser firefox** for running FireFox
* **--browser ie** for running Internet Explorer

* as an application parameter: **--credentials [userName] [password]**
* by storing user name and password as an **bossaCredentials** separated by semicolon:

## Credentials

User name and password to Bossa.pl account can be provided:

* as an application parameter: **--credentials [userName] [password]**
* as an **bossaCredentials** environment variable separated by semicolon

## Usage

Usage examples from F# or C# code are in [Sample](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples) folder:

* **[/NolRunnerAppFs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppFs)** folder contains F# application using this package
* **[/NolRunnerAppFs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppCs)** folder contains C# application using this package

After compilation from within:
 (***dotnet build***), the application should be used with parameters:
  
__dotnet run__ **--credentials [username] [password]**

  ![Parameters](docs/assets/FsBossaNolRunnerExe.png)
  
  If the parameter will not be provided, then ***user name*** and ***password*** will be taken from environment variable **'bossaCredentials'** :

  ![EnvironmentVariables](docs/assets/EnvironmentVariables.png)
  ![BossaCredentials](docs/assets/BossaCredentials.png)

If there are no credentials provided as an application parameter nor stored in environment variable then the application will not run neither browser nor NOL3.

The package was tested using:

* **Windows 10*** 1809
* **Chrome** 71.0.3578.98
* **FireFox** 64.0.2
* **Internet Explorer** 11
* **NOL 3** 3.1.15.191.I.7
* [**VS2017 15.9.5**](https://www.visualstudio.com/pl/downloads) (with F# Desktop Components)

*The package will not be actively maintained.*