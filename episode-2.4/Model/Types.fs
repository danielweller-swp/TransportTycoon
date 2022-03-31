module TransportTycoon.Model.Types

open TransportTycoon.Types
open NodaTime

type Road = Location * Location
type Model = Road -> LocalDateTime -> Speed

type TrainingData = LocalDateTime * Speed
