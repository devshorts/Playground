namespace CsvHandler

open System
open System.Reflection
open System.IO
open DataEmitter
open FSharp.Data.Csv

module CsvReader = 
    let rand = System.Random()

    let randomName() = rand.Next (0, 999999) |> string

    let defaultHeaders size = [0..size] |> List.map (fun i -> "Unknown Header " + (string i))

    let load (stream : Stream) = 
        let csv = CsvFile.Load(stream).Cache()

        let headers = match csv.Headers with 
                        | Some(h) -> h |> Array.toList
                        | None -> csv.NumberOfColumns |> defaultHeaders

        let fields = headers |> List.map (fun fieldName -> (fieldName, typeof<string>))

        let typeData = make (randomName()) fields

        [
            for item in csv.Data do       
                let paramsArr = item.Columns |> Array.map (fun i -> i :> obj)
                yield Activator.CreateInstance(typeData, paramsArr)         
        ]