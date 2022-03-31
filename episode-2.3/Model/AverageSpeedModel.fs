module TransportTycoon.Model.AverageSpeedModel

open TransportTycoon.Model.Types

let getModel (trainingData: Map<Road, TrainingData seq>) : Model =
    let model =
        trainingData
        |> Map.map(fun road data ->
            let speeds =
                data
                |> Seq.map (fun (_, speed) -> speed)
            Seq.sum(speeds) / (speeds |> Seq.length |> decimal) )

    fun road time -> model.Item road