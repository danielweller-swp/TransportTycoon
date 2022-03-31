namespace TransportTycoon.Simulation

module Types =
  open TransportTycoon.Types

  type Road = Location * Location * Length * Speed

  type Timestamp = int

  type ShortestPathContext = {
    Locations: Location list
    Roads: Road list
  }

  type Distance =
    | InfiniteDistance
    | FiniteDistance of Length