module TransportTycoon.Json

open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())


let serialize data = JsonSerializer.Serialize(data, options)
    
    
let deserialize<'T> (str: string) = JsonSerializer.Deserialize<'T>(str, options)