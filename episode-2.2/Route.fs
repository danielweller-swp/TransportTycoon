namespace TransportTycoon

open TransportTycoon.Types

module Route =

  type TimeLength = float
  
  type Duration =
    | InfiniteDuration
    | FiniteDuration of TimeLength
  
  type MilestoneStats = {
    Distance: Distance
    Duration: Duration
  }
  
  type Milestone = {
    Previous: Milestone option
    Location: Location
    MilestoneStats: MilestoneStats
  }
  
  let rec routeFromMilestone milestone =
    match milestone.Previous with
    | None -> [ milestone ]
    | Some previousMilestone -> routeFromMilestone previousMilestone @ [ milestone ]   
  
  type ShortestPath =
    | NoRoute
    | Route of Milestone list
  
  let rec shortestPathRec (current: Location) (destination: Location) (roads: Road list) (unvisited: Location list) (shortestKnownPaths: Map<Location, Milestone>) =
    let currentMilestone = shortestKnownPaths.[current]
    let currentStats = currentMilestone.MilestoneStats
    
    let timeToCurrent, lengthToCurrent = 
      match currentStats.Duration, currentStats.Distance with
      | FiniteDuration x, FiniteDistance y -> x, y
      | _ -> failwith "Duration and Distance to current should be finite"

    let neighbors =
      roads
      |> List.filter( fun (from, _, _, _) -> from = current )
      
    let unvisitedNeighborLengthsAndDurations =
      neighbors
      |> List.filter (fun (_, dest, _, _) -> unvisited |> List.contains dest)
      |> List.map (fun (_, dest, length, speed) ->
        let newLength = lengthToCurrent + length
        let newTime = timeToCurrent + (float length / float speed)
        (dest, (newLength, newTime)))
      |> Map.ofList

    let newShortestKnownPaths =
      shortestKnownPaths
      |> Map.map (fun location neighborMilestone ->
        let lengthAndDurationToNeighborOption = 
          unvisitedNeighborLengthsAndDurations
          |> Map.tryFind location

        let newMilestone length timeLength =
          {
            Location = location
            Previous = currentMilestone |> Some
            MilestoneStats = {
              Distance = FiniteDistance length
              Duration = FiniteDuration timeLength
            }
          }
        
        match neighborMilestone.MilestoneStats.Duration, lengthAndDurationToNeighborOption with
        | InfiniteDuration, Some (lengthToNeighbor, timeLengthToNeighbor) -> newMilestone lengthToNeighbor timeLengthToNeighbor
        | FiniteDuration currentTimeLength, Some (lengthToNeighbor, timeLengthToNeighbor) when timeLengthToNeighbor < currentTimeLength -> newMilestone lengthToNeighbor timeLengthToNeighbor
        | _, _ -> neighborMilestone)

    if current = destination then
      shortestKnownPaths.[current] |> routeFromMilestone |> Route
    else
      let newUnvisited = 
        unvisited
        |> List.except [ current ]

      let locationAndDurationComparer = fun (_, distance1) (_, distance2) -> 
          match (distance1, distance2) with
          | InfiniteDuration, _ -> 1
          | _, InfiniteDuration -> -1
          | FiniteDuration x, FiniteDuration y -> int (x - y)
      
      let next =
        newUnvisited
        |> List.map (fun l -> (l, newShortestKnownPaths.[l].MilestoneStats.Duration))
        |> List.sortWith locationAndDurationComparer
        |> List.head        

      match next with
      | _, InfiniteDuration -> NoRoute
      | location, _ -> shortestPathRec location destination roads newUnvisited newShortestKnownPaths

  let shortestPath context start destination =
    let unvisited = context.Locations
    let shortestKnownPaths =
      unvisited
      |> List.map( fun location ->
        let distance, duration =
          if location = start then
            FiniteDistance 0, FiniteDuration 0.0
          else
            InfiniteDistance, InfiniteDuration
        let milestone = {
          Location = location
          Previous = None
          MilestoneStats = {
            Distance = distance
            Duration = duration            
          }
        }
        (location, milestone))
      |> Map.ofList
    shortestPathRec start destination context.Roads unvisited shortestKnownPaths
