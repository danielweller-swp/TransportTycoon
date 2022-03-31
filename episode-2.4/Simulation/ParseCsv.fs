module TransportTycoon.Simulation.ParseCsv

open TransportTycoon.Types
open Types
open FSharp.Data

type CsvDistances = CsvProvider<"s02e02_map.csv">

let getDistanceData (file: string) =
    let data = CsvDistances.Load file
    let locations =
        data.Rows
        |> Seq.collect (fun row -> [ City row.A; City row.B ])

    let roads =
        data.Rows
        |> Seq.collect (fun row ->
            [
             (City row.A, City row.B, row.Km)
             (City row.B, City row.A, row.Km)
            ] )
        
    {
        Locations = locations |> List.ofSeq
        Roads = roads |> List.ofSeq
    }
