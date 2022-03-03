namespace TransportTycoon

module Types =

  type Location =
    | City of string

  type Length = int

  type Road = Location * Location * Length

  type Timestamp = int

  type ShortestPathContext = {
    Locations: Location list
    Roads: Road list
  }

  type Distance =
    | Infinite
    | Finite of Length