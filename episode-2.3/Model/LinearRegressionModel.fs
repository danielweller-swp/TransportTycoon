module TransportTycoon.Model.LinearRegressionModel

open MathNet.Numerics
open NodaTime
open TransportTycoon.Model.Types

let secondsFromMidnight (time: LocalDateTime) =
    (time.Hour * 60 + time.Minute) * 60 + time.Second

// Assumption: The time of day plays a linear role
// for the speed.
let getModel (trainingData: Map<Road, TrainingData seq>) : Model =
    let model =
        trainingData
        |> Map.map (fun road data ->
            let timesOfDay =
                data
                |> Seq.map (fun (time, _) -> time |> secondsFromMidnight |> float)
                |> Array.ofSeq
                
            let speeds =
                data
                |> Seq.map (fun (_, speed) -> speed |> float)
                |> Array.ofSeq
                
            let a, b = Fit.Line(timesOfDay, speeds)
            fun time ->
                let x = time |> secondsFromMidnight |> float
                a + b * x |> decimal
            )
    fun road time ->
        let modelForRoad = model.Item road
        modelForRoad time
        