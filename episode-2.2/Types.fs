namespace TransportTycoon

module Types =

  type Location =
    | City of string

  type Length = int
  
  type Speed = int

  type Road = Location * Location * Length * Speed

  type Timestamp = int

  type ShortestPathContext = {
    Locations: Location list
    Roads: Road list
  }

  type Distance =
    | InfiniteDistance
    | FiniteDistance of Length