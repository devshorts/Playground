module CsvHandler.Tests

open DataEmitter
open FsUnit
open System
open System.Reflection
open NUnit.Framework
open System.IO
open System

[<Test>]
let typeTest () = 
    let typeName = "typeName"
    let fields = [("intField", typeof<int>); ("stringField", typeof<string>)]
    let t = DataEmitter.make typeName fields

    let instance = Activator.CreateInstance(t, 1, "foo")

    let intField = t.GetField("intField")
    let stringField = t.GetField("stringField")

    intField.GetValue(instance) |> should equal 1
    stringField.GetValue(instance) |> should equal "foo"

[<Test>]
let instantiateTest() = 
    let typeName = "typeName2"
    let fields = 
        [ { Name = "foo"; Type = typeof<String>; Value = "bar" };
          { Name = "intFoo"; Type = typeof<int>; Value = 42 }]

    let instance = instantiate typeName fields

    let getVal field = instance.GetType().GetField(field.Name).GetValue(instance)

    let results = fields |> List.map getVal

    fields |> List.map (fun i -> i.Value)
           |> List.zip results
           |> List.iter (fun (expected, actual) -> expected |> should equal actual)

[<Test>]
let testCsv() = 
    use file = File.OpenRead "samplecsv1.csv"

    let data = CsvReader.load file

    ()