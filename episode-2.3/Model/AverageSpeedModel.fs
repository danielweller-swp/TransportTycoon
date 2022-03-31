module TransportTycoon.Model.AverageSpeedModel

open TransportTycoon.Types
open TransportTycoon.Model.Types

let getModel (trainingData: Map<Road, Speed seq>) : Model=
    trainingData
    |> Map.map(fun road speeds -> Seq.sum(speeds) / (speeds |> Seq.length |> decimal) )

