module TransportTycoon.Main

open Types
open Route

[<EntryPoint>]
let main argv =
    let context = ParseCsv.getData "s02e02_map.csv"
    
    let start = City argv.[0]
    let destination = City argv.[1]
    
    let extractFiniteDuration = function
        | FiniteDuration x -> x
        | _ -> failwith "Duration not finite"
        
    let extractCity = function
        | City x -> x
    
    let displayMilestone milestone =
        let what =
            match milestone.Previous with
            | None -> "DEPART"
            | _ -> "ARRIVE"
        $"%.2f{milestone.MilestoneStats.Duration |> extractFiniteDuration}h\t{what}\t{milestone.Location |> extractCity}"
    
    match shortestPath context start destination with
    | Route route ->
        let routeStr =
            route
            |> List.map displayMilestone
            |> String.concat "\n"
        System.Console.WriteLine(routeStr)
        0
    | NoRoute ->
        System.Console.WriteLine($"No path from {start} to {destination} found.")
        1
