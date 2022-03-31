module TransportTycoon.Main

[<EntryPoint>]
let main argv =
    let trainingData = TransportTycoon.Model.ParseCsv.getTrainingData "s02e03_train.csv"
    let testData = TransportTycoon.Validation.ParseCsv.getTestData "s02e03_test.csv"
    
    let averageSpeedModel = TransportTycoon.Model.AverageSpeedModel.getModel trainingData
    let averageSpeedModelError =
        TransportTycoon.Validation.MeanSquareError.modelError averageSpeedModel testData

    let linearModel = TransportTycoon.Model.LinearRegressionModel.getModel trainingData
    let linearModelError =
        TransportTycoon.Validation.MeanSquareError.modelError linearModel testData
    
    System.Console.WriteLine($"AverageSpeedModel MSE: {averageSpeedModelError}")
    System.Console.WriteLine($"LinearModel MSE: {linearModelError}")

    let polynomialOrders = [2; 3; 4; 5]
    
    polynomialOrders
    |> Seq.iter (fun order ->
        let model = TransportTycoon.Model.PolynomialRegressionModel.getModel order trainingData
        let error =
            TransportTycoon.Validation.MeanSquareError.modelError model testData
        System.Console.WriteLine($"PolynomialModel (order {order}) MSE: {error}")
        )
    
    0
