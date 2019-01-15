namespace BossaNolRunner

    module NolRunner = 

        open canopy.classic
        open canopy.types
        open System
        open System.Threading
        open Types
        open Utils
        open Argu
        open AppArguments
        open System.Diagnostics
        open Registry

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

        let startBrowser browserStartMode (credentials : Credentials)  =        
            match browserStartMode with
            | BrowserStartMode.Chrome 
            | BrowserStartMode.Firefox 
            | BrowserStartMode.IE ->
                consoleWriteLine "Starting browser ... " ConsoleColor.Blue
                start browserStartMode 
            | _ ->
                consoleWriteLine "Starting browser ... " ConsoleColor.Blue
                start chrome
     
            pin FullScreen
            url "https://www.bossa.pl/bossa/login"
            consoleWriteLine "Browser started!" ConsoleColor.Green
            credentials
    
        let login  (credentials : Credentials) =    
            consoleWriteLine "Login to bossa.pl starting..." ConsoleColor.Blue
            let username, password = credentials
            let loginString = 
                String.Format(" var f = document.forms.login; 
                    f.LgnUsrNIK.value='{0}'; 
                    f.LgnUsrPIN.value='{1}'; 
                    f.LgnUsrPIN.focus(); ", username, password)
            js loginString |> ignore
            press enter
            Thread.Sleep(TimeSpan.FromSeconds(3.0))
            consoleWriteLine "Login to bossa.pl finished!" ConsoleColor.Green
            username, password

        let initNol (credentials : Credentials) = 
            consoleWriteLine "Initializing Nol 3..." ConsoleColor.Blue
            js @" parent.initNol();" |> ignore    
            Thread.Sleep(TimeSpan.FromSeconds(5.0))
            credentials

        // main execution path
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
            quit()
         
        let parseArgs (argv : string[]) =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)    
            let parser = ArgumentParser.Create<CLIArguments>(programName = "dotnet run", errorHandler = errorHandler)
            let arguments = parser.ParseCommandLine argv   
            let username, password = arguments.GetResult(<@ Credentials @>, defaultValue = (String.Empty, String.Empty))
            let browser = arguments.GetResult(<@ Browser @>, defaultValue = "chrome")
            (browser, username, password)

        let startNol (browser : string) (username : string) (password : string) =            
            stopNol()            
            let browserStartMode =
                match browser with              
                | "chrome" -> BrowserStartMode.Chrome
                | "firefox" -> BrowserStartMode.Firefox
                | "ie" -> BrowserStartMode.IE
                | _ -> BrowserStartMode.Chrome
            let credentialsFromArguments = 
                if String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password) 
                then None
                else Some (username, password)            
            runNolWithOptionalCredentials browserStartMode credentialsFromArguments
         
        let startNolWithArgs (argv : string[]) =                  
            let browser, username, password = parseArgs argv
            startNol browser username password
            
