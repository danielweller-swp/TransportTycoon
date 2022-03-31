module TransportTycoon.Model.AverageSpeedModel

open TransportTycoon.Types
open TransportTycoon.Model.Types

type ModelData = Map<Road, Speed>

let trainModelData (trainingData: Map<Road, TrainingData seq>) : ModelData =
        trainingData
        |> Map.map(fun road data ->
            let speeds =
                data
                |> Seq.map (fun (_, speed) -> speed)
            Seq.sum(speeds) / (speeds |> Seq.length |> decimal) )

let modelFromData (modelData: ModelData) =
    fun road time -> modelData.Item road

let trainModel (trainingData: Map<Road, TrainingData seq>) : Model =
    trainModelData trainingData
    |> modelFromData
