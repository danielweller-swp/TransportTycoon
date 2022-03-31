module TransportTycoon.Model.LinearRegressionModel

open MathNet.Numerics
open NodaTime
open TransportTycoon.Model.Types

let secondsFromMidnight (time: LocalDateTime) =
    (time.Hour * 60 + time.Minute) * 60 + time.Second

let timeToModelInput = secondsFromMidnight >> float

// Assumption: The time of day plays a linear role
// for the speed.
let getModel (trainingData: Map<Road, TrainingData seq>) : Model =
    let model =
        trainingData
        |> Map.map (fun road data ->
            let timesOfDay =
                data
                |> Seq.map (fun (time, _) -> time |> timeToModelInput)
                |> Array.ofSeq
                
            let speeds =
                data
                |> Seq.map (fun (_, speed) -> speed |> float)
                |> Array.ofSeq
                
            let f = Fit.LineFunc(timesOfDay, speeds).Invoke
            timeToModelInput >> f >> decimal
            )
    fun road time ->
        let modelForRoad = model.Item road
        modelForRoad time
        