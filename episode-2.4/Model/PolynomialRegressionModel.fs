module TransportTycoon.Model.PolynomialRegressionModel

open MathNet.Numerics
open NodaTime
open TransportTycoon.Model.Types

let secondsFromMidnight (time: LocalDateTime) =
    (time.Hour * 60 + time.Minute) * 60 + time.Second

let timeToModelInput = secondsFromMidnight >> float
let modelOutputToSpeed = decimal

type PolynomialParameters = float seq

let evaluatePolynomial (factors: PolynomialParameters) (x: float) =
    factors
    |> Seq.mapi( fun i p -> p * (x ** (float i)))
    |> Seq.sum

type ModelData = Map<Road, PolynomialParameters>

let modelFromData (modelData: ModelData) =
    fun road time ->
        time
        |> timeToModelInput
        |> evaluatePolynomial (modelData.Item road)
        |> modelOutputToSpeed

// Assumption: The time of day plays a polynomial role
// for the speed.
let trainModelData (polynomialOrder: int) (trainingData: Map<Road, TrainingData seq>) : ModelData =
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
                
            Fit.Polynomial(timesOfDay, speeds, polynomialOrder)
            |> Seq.ofArray
            )

let trainModel (polynomialOrder: int) (trainingData: Map<Road, TrainingData seq>) : Model =
    trainModelData polynomialOrder trainingData
    |> modelFromData
