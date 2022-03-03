module TransportTycoon.Main

open Types
open Route

[<EntryPoint>]
let main argv =
    let context = ParseCsv.getData "s02e01_map.csv"
    
    let start = City argv.[0]
    let destination = City argv.[1]
    
    match shortestPath context start destination with
    | Route route ->
        let routeStr =
            route
            |> List.map (function | City x -> x)
            |> String.concat ","
        System.Console.WriteLine(routeStr)
        0
    | NoRoute ->
        System.Console.WriteLine($"No path from {start} to {destination} found.")
        1
