namespace ExamSystem

open System
open System.IO
open System.Net

open System.Net.Sockets

module Utils = 
    let applyTo funcs elem  = List.map(fun f -> f elem) funcs
    let applyTupleTo funcs elem = List.map (fun f -> f <|| elem) funcs

module NetworkUtils =
    
    let post msg (mailBox:Agent<_>) = mailBox.Post msg

    let postAsyncIgnore msg (mailBox:Agent<_>) = 
        mailBox.PostAndAsyncReply (fun chan -> msg, chan)
            |> Async.Ignore
            |> Async.Start

    let postAndAsyncReply msg (mailBox:Agent<_>) = mailBox.PostAndAsyncReply(fun channel -> msg channel)

    let start  (mailBox:Agent<_>) = mailBox.Start()    

    let closeClient (client:TcpClient) = client.Close()

    let writeToSocket (tcp:TcpClient) msg =  
        try          
            let stream = tcp.GetStream()

            stream.Write (msg, 0, Array.length msg)

            true
        with
            | exn -> 
                tcp |> closeClient
                false

    let strToBytes (str:string) = System.Text.ASCIIEncoding.ASCII.GetBytes (str.Trim() + Environment.NewLine)
    
    let broadcast clients msg : (FailedClients * SucceedClients) = 
        List.fold(fun (failed, succeeded) client ->                     
                        match writeToSocket client msg with
                            | true -> (failed, client::succeeded)
                            | false -> (client::failed, succeeded)) ([], []) clients

    let removeTcp connections client = List.filter ((<>) client) connections

    let rec monitor predicate onAction client  = 
        async {
            let! isConnected = predicate client
            if not isConnected then
                onAction client
            else 
                return! monitor predicate onAction client
        }

    let broadcastStr connections msg = msg + Environment.NewLine |> strToBytes |> broadcast connections 

    let writeStrToSocket client str = str |> strToBytes |> writeToSocket client

module Option = 
    let bindDo f = 
        function
            | Some(x) -> f x |> ignore
            | None ->  ()
    
[<AutoOpen>]
module ExamUtils = 
    let postToRoom agentRepo roomId msg = 
        List.tryFind (fst >> (=) roomId) agentRepo.Rooms
            |> Option.bindDo (snd >> NetworkUtils.post msg)
        
    let startRoom mailbox = snd >> NetworkUtils.start <| mailbox