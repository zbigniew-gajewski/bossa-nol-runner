// Learn more about F# at http://fsharp.org

open System
open BossaNolRunner.NolRunner

[<EntryPoint>]
let main argv =     
      
    // startNolWithArgs argv 

    let browser, username, password = parseArgs argv
    startNol browser username password

    Console.WriteLine("Press any key to continue...") 
    Console.ReadLine() |> ignore    

    stopNol ()

    0