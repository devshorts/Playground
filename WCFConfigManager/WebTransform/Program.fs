open FSharp.Data
open System.Collections.Generic
open System.Xml.Linq
open System.IO
open System.Xml
open Newtonsoft.Json
open FSharpx.Strings
open FSharpx.Collections

[<Literal>]
let sampleFile = "sample.xml"

type WebConfig = XmlProvider<sampleFile>

let save (path : string) (config : XElement) = path |> config.Save

type AppConfigs = {
    Services: Dictionary<string, string>
    DBs: Dictionary<string, string>
}

let jsonToConfig str = JsonConvert.DeserializeObject<AppConfigs>(str)

let updateAttribute attributeName attributeValue (xmlElement : XElement) = 
    xmlElement.Attribute(XName.Get(attributeName)).SetValue(attributeValue)

let lookup (config : Dictionary<string, string>) item =     
    let (found, newValue) = config.TryGetValue(item)     

    if found then Some newValue 
    else None

let replaceDataSourceWith newDbName sourceString  = 
    let splits = split ';' sourceString 
    let ``dataSource=`` = "Data Source="
    [
        yield ``dataSource=`` + newDbName

        for split in splits do
            if not (startsWith ``dataSource=`` split) then 
                yield split
    ] 
        |> Seq.intersperse ";"
        |> Seq.reduce (+)

let findEndPoints (config: AppConfigs) (w: WebConfig.Configuration)  = 
    try
        [        
            for endpoint in w.SystemServiceModel.Client.Endpoints do                               
                yield endpoint        
        ]
    with
        | ex -> 
            printfn "no endpoints found"
            []

let findDatabases (config: AppConfigs) (w: WebConfig.Configuration)  = 
    try
        [
            for connectionString in w.ConnectionStrings.Adds do
                yield connectionString
        ]
    with
        | ex -> 
            printfn "No connection strings found"
            []

let newEndpointValues config (endpoints: WebConfig.Endpoint list) = 
    [
        for endpoint in endpoints do        
            match endpoint.Contract |> lookup config.Services with
                | Some(newAddress) ->  yield (endpoint.XElement, newAddress)
                | _ -> ()
    ]

let newDatabaseValues config (connectionStrings: WebConfig.Add2 list) = 
    [
        for connectionString in connectionStrings do 
            match connectionString.Name |> lookup config.DBs with
                | Some(sourceDB) when not (isNullOrEmpty sourceDB) -> 
                    let mergedConnectionString = connectionString.ConnectionString |> replaceDataSourceWith sourceDB
                    yield (connectionString.XElement, mergedConnectionString)
                | _ -> ()   
    ]

let modifyXmlAttribute attribute (xmlElement, newValue) = xmlElement |> updateAttribute attribute newValue

let updateEndpoints (xml: WebConfig.Configuration) (config: AppConfigs) = 
    xml |> findEndPoints config
        |> newEndpointValues config
        |> List.iter (modifyXmlAttribute "address")

let updateConnections (xml: WebConfig.Configuration) (config: AppConfigs) =
    xml |> findDatabases config
        |> newDatabaseValues config
        |> List.iter (modifyXmlAttribute "connectionString")


let sampleJson = @"
{ 
    ""Services"": { 
        ""Test.Mgmt.Users"" : ""http://localhost/Users.svc"" 
    }, 
    ""DBs"": { 
        ""DBName1"":""localhost"",
        ""DBName2"": ""localhost1"",
        ""SomeOtherDb"" : ""localhost2"" 
    } 
}"

[<EntryPoint>]
let main argv = 
    
    if Array.length argv <> 2 then 
        printfn "Usage: <deployed root> <json config path>"
        printfn ""
        printfn "Json format:"
        printfn ""
        printfn "%s" sampleJson
        1
    else
        let root = argv.[0]
        let config = argv.[1] |> File.ReadAllText |> jsonToConfig

        for file in Directory.EnumerateFiles(root, "Web.config", SearchOption.AllDirectories) do
            try
                let webXml = WebConfig.Load file
                updateEndpoints webXml config
                updateConnections webXml config
                webXml.XElement |> save file
                printfn "Updated %s" file
            with
                | ex -> printfn "Failed to modify file %s: %A" file ex
        0 // return an integer exit code
