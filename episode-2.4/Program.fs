module TransportTycoon.Main

[<Literal>]
let TrainCommand = "train_model"

[<Literal>]
let SimulationCommand = "run_simulation"

[<EntryPoint>]
let main argv =
    (*
    match argv.[0] with
    | TrainCommand ->
        System.Console.WriteLine("training")
        0
    | SimulationCommand ->
        System.Console.WriteLine("simulation")
        0
    | x ->
        System.Console.WriteLine($"Unknown command {x}")
        1
    *)
    let trainingData = TransportTycoon.Model.ParseCsv.getTrainingData "s02e03_train.csv"
    let testData = TransportTycoon.Validation.ParseCsv.getTestData "s02e03_test.csv"

    let averageSpeedModel = TransportTycoon.Model.AverageSpeedModel.trainModel trainingData
    let linearModel = TransportTycoon.Model.LinearRegressionModel.trainModel trainingData

    let polynomialOrders = [2; 3; 4; 5]
    
    let polynomialModels =
        polynomialOrders
        |> Seq.map (fun order ->
            let model = TransportTycoon.Model.PolynomialRegressionModel.trainModel order trainingData
            $"Polynomial Regression (order {order})", model)

    let models =
        [ "Average Speed", averageSpeedModel
          "Linear Regression", linearModel ]
        |> Seq.append polynomialModels

    let evaluatedModels =
        models
        |> Seq.map( fun (name, model) ->
            (name, model, TransportTycoon.Validation.MeanSquareError.modelError model testData))

    evaluatedModels
    |> Seq.iter(fun (name, model, error) ->
        System.Console.WriteLine($"{name} MSE: {error}"))

    let bestModel, _, _ =
        evaluatedModels
        |> Seq.minBy (fun (name, model, error) -> error)

    System.Console.WriteLine($"Best Model: {bestModel}")

    0
