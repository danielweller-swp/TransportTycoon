module TransportTycoon.Model.LinearRegressionModel

open MathNet.Numerics
open NodaTime
open TransportTycoon.Model.Types

let secondsFromMidnight (time: LocalDateTime) =
    (time.Hour * 60 + time.Minute) * 60 + time.Second

let timeToModelInput = secondsFromMidnight >> float
let modelOutputToSpeed = decimal

type LinearParameters = {
    A: float
    B: float
}

type ModelData = Map<Road, LinearParameters>

let modelFromData (modelData: ModelData) =
    fun road time ->
        let param = modelData.Item road
        time
        |> timeToModelInput
        |> fun x -> param.A + param.B * x
        |> modelOutputToSpeed

// Assumption: The time of day plays a linear role
// for the speed.
let trainModelData (trainingData: Map<Road, TrainingData seq>) : ModelData =
        trainingData
        |> Map.map (fun _ data ->
            let timesOfDay =
                data
                |> Seq.map (fun (time, _) -> time |> timeToModelInput)
                |> Array.ofSeq
                
            let speeds =
                data
                |> Seq.map (fun (_, speed) -> speed |> float)
                |> Array.ofSeq
                
            let a, b = Fit.Line(timesOfDay, speeds)
            {
                A = a
                B = b
            }
            )

let trainModel (trainingData: Map<Road, TrainingData seq>) : Model =
    trainModelData trainingData
    |> modelFromData
