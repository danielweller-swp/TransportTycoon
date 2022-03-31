module TransportTycoon.Validation.ParseCsv

open NodaTime
open TransportTycoon.Types
open Types
open FSharp.Data

type CsvTestData = CsvProvider<"s02e03_test.csv">

let getTestData (file: string) : TestDataRow seq =
    let data = CsvTestData.Load file

    data.Rows
    |> Seq.map( fun d -> ((City d.A, City d.B), d.TIME |> LocalDateTime.FromDateTime, d.SPEED))