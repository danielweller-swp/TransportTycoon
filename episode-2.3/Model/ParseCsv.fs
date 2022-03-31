module TransportTycoon.Model.ParseCsv

open NodaTime
open TransportTycoon.Types
open TransportTycoon.Model.Types
open FSharp.Data

type CsvTraining = CsvProvider<"s02e03_train.csv">
type Road = Location * Location

let getTrainingData (file: string) : Map<Road, TrainingData seq> =
    let data = CsvTraining.Load file
    
    data.Rows
    |> Seq.map( fun d -> ((City d.A, City d.B), (d.TIME, d.SPEED) ))
    |> Seq.groupBy( fun (road, speed) -> road )
    |> Map.ofSeq
    |> Map.map(
        fun road data ->
            data
            |> Seq.map (fun (road, (time, speed)) -> (LocalDateTime.FromDateTime time, speed) )
            )

type Model = Map<Road, Speed>

