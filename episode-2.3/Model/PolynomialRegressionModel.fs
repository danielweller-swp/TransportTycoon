module TransportTycoon.Model.PolynomialRegressionModel

open MathNet.Numerics
open NodaTime
open TransportTycoon.Model.Types

let secondsFromMidnight (time: LocalDateTime) =
    (time.Hour * 60 + time.Minute) * 60 + time.Second

let evaluatePolynomial (factors: float seq) (x: float) =
    factors
    |> Seq.mapi( fun i p -> p * (x ** (float i)))
    |> Seq.sum

// Assumption: The time of day plays a polynomial role
// for the speed.
let getModel (polynomialOrder: int) (trainingData: Map<Road, TrainingData seq>) : Model =
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
                
            let factors = Fit.Polynomial(timesOfDay, speeds, polynomialOrder)
            fun time ->
                let x = time |> secondsFromMidnight |> float
                evaluatePolynomial factors x |> decimal
            )
    fun road time ->
        let modelForRoad = model.Item road
        modelForRoad time
        