module TransportTycoon.Validation.ParseCsv

open NodaTime
open TransportTycoon.Types
open Types
open FSharp.Data

type CsvTestData = CsvProvider<"s02e04_test.csv">

let getTestData (file: string) : TestDataRow seq =
    let data = CsvTestData.Load file

    data.Rows
    |> Seq.map( fun d -> {
        Road = (City d.A, City d.B)
        Start = d.START |> LocalDateTime.FromDateTime
        End = d.END |> LocalDateTime.FromDateTime
        Speed = d.SPEED })