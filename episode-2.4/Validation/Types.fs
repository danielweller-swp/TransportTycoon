module TransportTycoon.Validation.Types

open NodaTime
open TransportTycoon.Types
open TransportTycoon.Model.Types

type TestDataRow = {
    Road: Road
    Start: LocalDateTime
    End: LocalDateTime
    Speed: Speed
}

type ValidationResult = decimal
