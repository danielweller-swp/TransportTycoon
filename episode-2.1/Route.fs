namespace TransportTycoon

module Route =

  open Types

  type Milestone = {
    Previous: Milestone option
    Location: Location
    Distance: Distance
  }
  
  let rec routeFromMilestone milestone =
    match milestone.Previous with
    | None -> [ milestone.Location ]
    | Some previousMilestone -> routeFromMilestone previousMilestone @ [ milestone.Location ]   
  
  type ShortestPath =
    | NoRoute
    | Route of Location list
  
  let rec shortestPathRec (current: Location) (destination: Location) (roads: Road list) (unvisited: Location list) (shortestKnownPaths: Map<Location, Milestone>) =
    let lengthToCurrent = 
      match shortestKnownPaths.[current].Distance with
      | Finite x -> x
      | _ -> failwith "Distance to current should be finite"

    let neighbors =
      roads
      |> List.filter( fun (from, _, _) -> from = current )
      
    let unvisitedNeighborLengths =
      neighbors
      |> List.filter (fun (_, dest, _) -> unvisited |> List.contains dest)
      |> List.map (fun (_, dest, length) -> (dest, lengthToCurrent + length))
      |> Map.ofList

    let newShortestKnownPaths =
      shortestKnownPaths
      |> Map.map (fun location neighborMilestone ->
        let currentMilestone = shortestKnownPaths.[current]
        let lengthToNeighborOption = 
          unvisitedNeighborLengths
          |> Map.tryFind location

        let newMilestone length =
          {
            Location = location
            Previous = currentMilestone |> Some
            Distance = Finite length
          }
        
        match neighborMilestone.Distance, lengthToNeighborOption with
        | Infinite, Some lengthToNeighbor -> newMilestone lengthToNeighbor
        | Finite currentLength, Some lengthToNeighbor when lengthToNeighbor < currentLength -> newMilestone lengthToNeighbor
        | _, _ -> neighborMilestone )

    if current = destination then
      shortestKnownPaths.[current] |> routeFromMilestone |> Route
    else
      let newUnvisited = 
        unvisited
        |> List.except [ current ]

      let locationAndDistanceComparer = fun (_, distance1) (_, distance2) -> 
          match (distance1, distance2) with
          | Infinite, _ -> 1
          | _, Infinite -> -1
          | Finite x, Finite y -> x - y
      
      let next =
        newUnvisited
        |> List.map (fun l -> (l, newShortestKnownPaths.[l].Distance))
        |> List.sortWith locationAndDistanceComparer
        |> List.head        

      match next with
      | _, Infinite -> NoRoute
      | location, _ -> shortestPathRec location destination roads newUnvisited newShortestKnownPaths

  let shortestPath context start destination =
    let unvisited = context.Locations
    let shortestKnownPaths =
      unvisited
      |> List.map( fun location ->
        let distance =
          if location = start then
            Finite 0
          else
            Infinite
        let milestone = {
          Location = location
          Previous = None
          Distance = distance
        }
        (location, milestone))
      |> Map.ofList
    shortestPathRec start destination context.Roads unvisited shortestKnownPaths
