module TransportTycoon.Main

[<EntryPoint>]
let main argv =
    let trainingData = TransportTycoon.Model.ParseCsv.getTrainingData "s02e03_train.csv"
    let testData = TransportTycoon.Validation.ParseCsv.getTestData "s02e03_test.csv"

    let averageSpeedModel = TransportTycoon.Model.AverageSpeedModel.getModel trainingData
    let linearModel = TransportTycoon.Model.LinearRegressionModel.getModel trainingData

    let polynomialOrders = [2; 3; 4; 5]
    
    let polynomialModels =
        polynomialOrders
        |> Seq.map (fun order ->
            let model = TransportTycoon.Model.PolynomialRegressionModel.getModel order trainingData
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
