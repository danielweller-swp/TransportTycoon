module TransportTycoon.Model.Types

open TransportTycoon.Types

type Road = Location * Location
type Model = Map<Road, Speed>

