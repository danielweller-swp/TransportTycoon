module TransportTycoon.Validation.Types

open NodaTime
open TransportTycoon.Types
open TransportTycoon.Model.Types

type TestDataRow = Road * LocalDateTime * Speed
type ValidationResult = decimal
