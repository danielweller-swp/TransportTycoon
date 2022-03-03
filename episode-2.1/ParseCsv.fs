module TransportTycoon.ParseCsv

open Types
open FSharp.Data

type CsvDistances = CsvProvider<"s02e01_map.csv">

let getData (file: string) =
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
