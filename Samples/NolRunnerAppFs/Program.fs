// Learn more about F# at http://fsharp.org

open System
open BossaNolRunner
open BossaNolRunner.NolRunner

[<EntryPoint>]
let main argv =   
    
    startNol argv 
    
    Console.WriteLine("Press any key to continue...") 
    Console.ReadLine() |> ignore    

    stopNol ()

    0