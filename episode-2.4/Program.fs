module TransportTycoon.Main

open System.IO
open TransportTycoon.Json
open TransportTycoon.Model
open TransportTycoon.Model.Types

[<Literal>]
let AverageModelCommand = "average"

[<Literal>]
let LinearModelCommand = "linear"

[<Literal>]
let PolynomialModelCommand = "polynomial"

let PolynomialOrder = 3

[<Literal>]
let TrainCommand = "train_model"

[<Literal>]
let SimulationCommand = "run_simulation"

[<Literal>]
let ModelDataOutputFile = "model.json"

let save file str = File.WriteAllText(file, str)

let load file = File.ReadAllText(file)

let trainAndSave trainingData file = function
    | AverageModelCommand ->
        AverageSpeedModel.trainModelData trainingData |> serialize |> save file
    | LinearModelCommand ->
        LinearRegressionModel.trainModelData trainingData |> serialize |> save file
    | PolynomialModelCommand ->
        PolynomialRegressionModel.trainModelData PolynomialOrder trainingData |> serialize |> save file
    | x -> failwith $"Unknown model type {x}"

let modelFromFile file = function
    | AverageModelCommand ->
        load file |> deserialize |> AverageSpeedModel.modelFromData
    | LinearModelCommand ->
        load file |> deserialize |> LinearRegressionModel.modelFromData
    | PolynomialModelCommand ->
        load file |> deserialize |> PolynomialRegressionModel.modelFromData
    | x -> failwith $"Unknown model type {x}"
    

[<EntryPoint>]
let main argv =
    let command = argv.[0]
    let modelType = argv.[1]
    
    match command with
    | TrainCommand ->
        let modelDataFile = argv.[2]
        let trainingData = ParseCsv.getTrainingData modelDataFile
        trainAndSave trainingData ModelDataOutputFile modelType
        System.Console.WriteLine($"Saved to {ModelDataOutputFile}")
        0
    | SimulationCommand ->
        let modelFile = argv.[2]
        let model = modelFromFile modelFile modelType
        
        let testDataFile = argv.[3]
        let testData = Validation.ParseCsv.getTestData testDataFile
        
        let context = Simulation.ParseCsv.getDistanceData "s02e02_map.csv"
        
        let mse = Validation.MeanSquareError.modelBasedSimulationError model context testData
        System.Console.WriteLine($"Simulation MSE is {mse}")
        0
    | x ->
        System.Console.WriteLine($"Unknown command {x}")
        1
