namespace CsvHandler

open DataEmitter
open System.Management.Automation
open System.Reflection
open System
open System.IO

[<Cmdlet("Read", "Csv")>]
type CsvParser() =
    inherit PSCmdlet()
    
    [<Parameter(Position = 0)>]
    member val File : string = null with get, set

    override this.ProcessRecord() = 
        let (fileNames, _) = this.GetResolvedProviderPathFromPSPath this.File
             
        for file in fileNames do   
            use fileStream = File.OpenRead file

            fileStream 
                |> CsvReader.load 
                |> List.toArray 
                |> this.WriteObject