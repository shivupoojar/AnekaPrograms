// Learn more about F# at http://fsharp.org

open System
open Aneka
open Aneka.Threading
open Aneka.Entity

let AnekaApplication<AnekaThread, ThreadManager> app = null

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
