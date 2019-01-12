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

**'--browser'** parameter is optionl, the default is **Chrome**.

## Credentials

User name and password to Bossa.pl account can be provided:

* as an application parameter: **--credentials [userName] [password]**
* as an **bossaCredentials** environment variable separated by semicolon:
  
  ![EnvironmentVariables](docs/assets/EnvironmentVariables.png)

  ![BossaCredentials](docs/assets/BossaCredentials.png)


## Usage

When you are using this package in your project and want to run or debug your application from Visual Studio or from Visual Studio Code, remember to add a tag **CopyLocalLockFilesAssemblies** with **true** setting to **PropertyGroup** element:

```<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
```

This is because canopy library requires to have browser driver in the same location. An example project should look like this:

```<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BossaNolRunner" Version="1.0.0" />
    <PackageReference Include="Selenium.WebDriver.ChromeDriver" Version="2.45.0" />
  </ItemGroup>
</Project>
```



Usage examples from F# or C# are in [Sample](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples) folder:

* **[Sample/NolRunnerAppFs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppFs)** folder contains F# application using this package
* **[Sample/NolRunnerAppFs](https://github.com/zbigniew-gajewski/bossa-nol-runner/tree/master/Samples/NolRunnerAppCs)** folder contains C# application using this package

After compilation from within corresponding folder (NolRUnnerAppFs or NoRunnerAppCs):
  
  **dotnet build**

  the application can by run with parameters:

  **dotnet  run   --browser  chrome   --credentials  [username]  [password]**

  ![Parameters](docs/assets/FsBossaNolRunnerExe.png)
  
  If the parameter are not provided then ***user name*** and ***password*** will be taken from environment variable **'bossaCredentials'**. If there are no credentials provided as an application parameter nor stored in environment variable then the application will not run neither browser nor NOL3.

The package was tested using:

* **Windows 10** 1809
* **Chrome** 71.0.3578.98
* **FireFox** 64.0.2
* **Internet Explorer** 11
* **NOL 3** 3.1.15.191.I.7
* [**VS2017 15.9.5**](https://www.visualstudio.com/pl/downloads) (with F# Desktop Components)

*The package will not be actively maintained.*