namespace BossaNolRunner

    module NolRunner = 

        open System
        open System.Threading
        open System.Threading.Tasks
        open Types
        open Utils
        open Argu
        open AppArguments
        open System.Diagnostics
        open Registry
        open Microsoft.Playwright

        let validateEmptyCredentials credentials = 
            let username, password = credentials
            if String.IsNullOrEmpty(username) then Failure (EmptyUserNameException, None)
            elif String.IsNullOrEmpty(password) then Failure (EmptyPasswordException, None)
            else Success credentials  

        let validateInitialCredentials (optionalCredentials : Credentials Option) =
            consoleWriteLine "Validating initial credentials..." ConsoleColor.Blue
            match optionalCredentials with
            | Some credentials -> validateEmptyCredentials credentials
            | None -> Failure (NoInitialCredentialsException, None)

        let getVariablesFromEnvironment () = 
            let bossaCredentials = Environment.GetEnvironmentVariable("bossaCredentials", EnvironmentVariableTarget.User)
            let username = bossaCredentials.Split(';').[0]
            let password = bossaCredentials.Substring(username.Length + 1)  // password might contain semicolons
            username, password    

        let getCredentialsFromEnvironmentVariables getVariables (_ : Credentials Option)  =
            // For security reasons keep username and password for your bossa account in environment variable 'bossaCredentials':
            //   Variable: bossaCredentials
            //   Value: user89098;tajneHaslo123 (user name first then password separated by semicolon ';')
            consoleWriteLine "Getting credentials from Environment variables..." ConsoleColor.Blue
            let credentialsFromEnvironment = getVariables ()
            validateEmptyCredentials credentialsFromEnvironment
    
        let validateCredentials getVariables = 
            validateInitialCredentials
            ||| 
            (getCredentialsFromEnvironmentVariables getVariables)

        let web = 
                Playwright.CreateAsync()
                |> Async.AwaitTask
                |> Async.RunSynchronously

        let mutable browser = 
            web.Chromium.LaunchAsync(BrowserTypeLaunchOptions(Headless = true))
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let startBrowser browserStartMode (credentials : Credentials) : (IPage * Credentials)  =
            
            consoleWriteLine "Starting browser ... " ConsoleColor.Blue
            
            let b =
                match browserStartMode with
                | "chrome" -> web.Chromium.LaunchAsync(BrowserTypeLaunchOptions(Headless = false))
                | "firefox" -> web.Firefox.LaunchAsync(BrowserTypeLaunchOptions(Headless = false))
                | "webkit" -> web.Webkit.LaunchAsync(BrowserTypeLaunchOptions(Headless = false))
                | _ -> web.Chromium.LaunchAsync(BrowserTypeLaunchOptions(Headless = false))
                |> Async.AwaitTask
                |> Async.RunSynchronously 

            browser.CloseAsync() |> Async.AwaitTask |> Async.RunSynchronously

            browser <- b

            let page = 
                browser.NewPageAsync()
                |> Async.AwaitTask
                |> Async.RunSynchronously
            
            consoleWriteLine "Browser started!" ConsoleColor.Green

            page, credentials



        let login ((page : IPage), (credentials : Credentials)) : (IPage * Credentials) = 

            consoleWriteLine "Login to bossa.pl starting..." ConsoleColor.Blue

            page.GotoAsync(@"https://www.bossa.pl/bossa/login") |> Async.AwaitTask |> Async.RunSynchronously |> ignore
 
            let username, password = credentials
            page.FillAsync("input[name='login']", username) |> Async.AwaitTask |> Async.RunSynchronously
            page.FillAsync("input[name='password']", password) |> Async.AwaitTask |> Async.RunSynchronously
            page.ClickAsync("button[name='buttonLogin']") |> Async.AwaitTask |> Async.RunSynchronously
            
            consoleWriteLine "Login to bossa.pl successful!" ConsoleColor.Green

            page, credentials

            // pin FullScreen
            // url 
         
            //credentials
    
        let initNol ((page : IPage), (credentials : Credentials))  = 
            consoleWriteLine "Initializing Nol 3..." ConsoleColor.Blue
            page.ClickAsync("id=skipPollButtonAntrd") |> Async.AwaitTask |> Async.RunSynchronously |> ignore
            page.ClickAsync("id=confirmPreambleSkipAntrd") |> Async.AwaitTask |> Async.RunSynchronously |> ignore
            page.ClickAsync("id=hora2") |> Async.AwaitTask |> Async.RunSynchronously |> ignore
            page.EvaluateAsync<string>("javascript:parent.initNol();") |> Async.AwaitTask |> Async.RunSynchronously |> ignore
            consoleWriteLine "Login to bossa.pl finished!" ConsoleColor.Green
            consoleWriteLine "Nol 3 initialization finished." ConsoleColor.Blue
            page, credentials

        // *******************************************************
        // main execution path
        // *******************************************************

        let runNolWithOptionalCredentials browser (credentials : Credentials Option) : unit =   
    
            let executionChain =         
                (validateCredentials getVariablesFromEnvironment)
                >=> tryCatch (startBrowser browser) StartingBrowserException
                >=> tryCatch login LoginException
                >=> tryCatch initNol InitializingNolException
   
            let executionResult = credentials |> executionChain

            match executionResult with
            | Success (username, _) -> 
                consoleWriteLine (String.Format("Nol 3 initialized successfuly for user [{0}]!", username)) ConsoleColor.Green
            | Failure fullException -> 
                let printError message = 
                    consoleWriteLine (String.Format("Initializing Nol 3 failed!{0}{1}", Environment.NewLine, message )) ConsoleColor.Red
                match fullException with
                | EmptyUserNameException, _ -> printError "User Name can't be empty!"
                | EmptyPasswordException, _ -> printError "Password can't be empty!"
                | NoInitialCredentialsException, _ -> printError "No initial credentials provided!"
                | StartingBrowserException, ex -> printError (String.Format("Error when starting browser!{0}{1}", Environment.NewLine, ex.ToString()))  
                | LoginException, ex -> printError (String.Format("Error when login to bossa account!{0}{1}", Environment.NewLine, ex.ToString()))
                | InitializingNolException, ex -> printError (String.Format("Error when initializing Nol 3!{0}{1}", Environment.NewLine, ex.ToString()))

        
        let stopNol () = 
            Process.GetProcessesByName("NOL3") |> Array.iter (fun bossaNolProcess -> bossaNolProcess.Kill())
            setRegistryValue SYNCPORTSETKEY "0"
            Thread.Sleep(TimeSpan.FromSeconds(1.0))
            Process.GetProcessesByName("NOL3") |> Array.iter (fun bossaNolProcess -> bossaNolProcess.Kill())
            browser.CloseAsync() |> Async.AwaitTask |> Async.RunSynchronously
         
        let parseArgs (argv : string[]) =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)    
            let parser = ArgumentParser.Create<CLIArguments>(programName = "dotnet run", errorHandler = errorHandler)
            let arguments = parser.ParseCommandLine argv   
            let username, password = arguments.GetResult(<@ Credentials @>, defaultValue = (String.Empty, String.Empty))
            let browser = arguments.GetResult(<@ Browser @>, defaultValue = "chrome")
            (browser, username, password)

        let startNol (browser : string) (username : string) (password : string) =            
            stopNol()            
            let credentialsFromArguments = 
                if String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password) 
                then None
                else Some (username, password)            
            runNolWithOptionalCredentials browser credentialsFromArguments
         
        let startNolWithArgs (argv : string[]) =                  
            Microsoft.Playwright.Program.Main [|"install"|] |> ignore
            let browser, username, password = parseArgs argv
            startNol browser username password
            
