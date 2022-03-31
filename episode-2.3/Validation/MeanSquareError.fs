module TransportTycoon.Validation.MeanSquareError

open TransportTycoon.Model.Types
open Types

let modelErrorForRow (model: Model) (row: TestDataRow) =
    let road, actualSpeedDecimal = row
    let actualSpeed = actualSpeedDecimal |> float
    let predictedSpeed = model.Item road |> float
    let error = (actualSpeed - predictedSpeed) ** 2.0
    
    //System.Console.WriteLine($"{road}\t{predictedSpeed}\t{actualSpeed}\t{error}")
    
    error

let rec modelError (model: Model) (testData: TestDataRow seq) =
    //System.Console.WriteLine("ROAD\tPREDICATED SPEED\tACTUAL SPEED\tERROR")
    let errorSum =
        testData
        |> Seq.map (modelErrorForRow model)
        |> Seq.sum
        
    errorSum / (testData |> Seq.length |> float)