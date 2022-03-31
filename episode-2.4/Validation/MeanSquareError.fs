module TransportTycoon.Validation.MeanSquareError

open NodaTime
open Types
open TransportTycoon.Model.Types
open TransportTycoon.Simulation.Types
open TransportTycoon.Simulation.Route

let modelErrorForRow (model: Model) (row: TestDataRow) =
    let predictedSpeed = model row.Road row.Start
    let error = (row.Speed - predictedSpeed) * (row.Speed - predictedSpeed)
    
    //System.Console.WriteLine($"{road}\t{predictedSpeed}\t{actualSpeed}\t{error}")
    
    error

let modelError (model: Model) (testData: TestDataRow seq) =
    //System.Console.WriteLine("ROAD\tPREDICATED SPEED\tACTUAL SPEED\tERROR")
    let errorSum =
        testData
        |> Seq.map (modelErrorForRow model)
        |> Seq.sum
        
    errorSum / (testData |> Seq.length |> decimal)

let extractArrival (route: Milestone list) =
    let lastMilestone = route |> List.last
    match lastMilestone.MilestoneStats.Duration with
    | InfiniteDuration -> failwith "Unexpected infinite duration"
    | ArrivedAt time -> time 

let modelBasedSimulationErrorForRow (model: Model) (context: ShortestPathContext) (row: TestDataRow) =
    let actualTimeOfArrival = row.End
    let start, dest = row.Road
    
    let expectedTimeOfArrival =
        match shortestPath context model start row.Start dest with
        | NoRoute -> failwith $"No route found from {start} to {dest}"
        | Route route -> extractArrival route
    
    let error = Period.Between(actualTimeOfArrival, expectedTimeOfArrival, PeriodUnits.Minutes).Minutes
    let squared = error * error
    
    System.Console.WriteLine($"{row.Road}\t{expectedTimeOfArrival}\t{actualTimeOfArrival}\t{squared}")
    
    squared

let modelBasedSimulationError (model: Model) (context: ShortestPathContext) (testData: TestDataRow seq) =
    System.Console.WriteLine("ROAD\tETA\tATA\tERROR")
    let errorSum =
        testData
        |> Seq.map (modelBasedSimulationErrorForRow model context)
        |> Seq.sum
        
    (errorSum |> float) / (testData |> Seq.length |> float)