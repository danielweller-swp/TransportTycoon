module TransportTycoon.Main

[<EntryPoint>]
let main argv =
    let trainingData = TransportTycoon.Model.ParseCsv.getTrainingData "s02e03_train.csv"
    let model = TransportTycoon.Model.AverageSpeedModel.getModel trainingData
    
    let testData = TransportTycoon.Validation.ParseCsv.getTestData "s02e03_test.csv"
    let modelError = TransportTycoon.Validation.MeanSquareError.modelError model testData

    System.Console.WriteLine($"MSE is {modelError}")
    0
