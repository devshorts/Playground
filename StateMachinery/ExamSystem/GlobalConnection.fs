namespace ExamSystem

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Text
open System.Threading
open System.Runtime.Serialization
open ExamSystem
open ExamSystem.ExamControlData
open ExamSystem.StateManager
open ExamSystem.Utils
open ExamSystem.NetworkUtils
open ExamSystem.CommunicationProtocol

module GlobalConnection = 

    /// The global agent that can rebroadcast to all room controllers
    /// as well as the control interfaces (basically anyone connected)
    let globalAgent agentRepo = 
        Agent<GlobalMsg>.Start(fun inbox ->

            let applyToRooms msg = agentRepo().Rooms |> List.map snd |> List.iter msg

            let rec loop () = 
                async{
                    let! msg = inbox.Receive()
                    match msg with
                        | GlobalMsg.Broadcast str -> 
                            agentRepo().Control |> post (ControlInterfaceMsg.Broadcast str)
                            applyToRooms (post (RoomConnMsg.Broadcast str)) 

                    return! loop()
                }
            loop()
        )

    let findControllerForRoom roomId roomControllers = List.tryFind (fst >> (=) roomId) roomControllers

    let listenForControlConnections agentRepo = 
        let listener = new TcpListener(IPAddress.Any, 82)
        let cts = new CancellationTokenSource()
        let token = cts.Token
     
        let main = async {
            try
                listener.Start(10)
                while not cts.IsCancellationRequested do
                    let! client = Async.FromBeginEnd(listener.BeginAcceptTcpClient, listener.EndAcceptTcpClient)
                    printfn "Got client %s" <| client.Client.RemoteEndPoint.ToString()
                
                    agentRepo.Control |> post (ControlInterfaceMsg.Connect client)
                    agentRepo.Control |> post (ControlInterfaceMsg.Broadcast "control connnected")    
                   
            finally
                printfn "Listener stopping"
                listener.Stop()        
        }
 
        Async.Start(main, token)
 
        { 
            new IDisposable 
            with member x.Dispose() = 
                    cts.Cancel() |> ignore
                    cts.Dispose()
                    agentRepo.Control |> post ControlInterfaceMsg.Shutdown
        }

    /// Accepts sockets and hands off the connected client
    /// To the right agent based on their handshake
    let listenForRoomConnections agentRepo = 
        let listener = new TcpListener(IPAddress.Any, 81)
        let cts = new CancellationTokenSource()
        let token = cts.Token
     
        let main = async {
            try
                listener.Start(10)
                while not cts.IsCancellationRequested do
                    let! client = Async.FromBeginEnd(listener.BeginAcceptTcpClient, listener.EndAcceptTcpClient)
                    printfn "Got client %s" <| client.Client.RemoteEndPoint.ToString()
                
                    async {
                        "Enter room number: " |> writeStrToSocket client |> ignore

                        let roomId = roomNum client

                        agentRepo.Rooms
                            |> findControllerForRoom roomId
                            |> function
                                | Some room -> room |> snd |> post (RoomConnMsg.Connect client)
                                | None -> agentRepo.Global |> post (GlobalMsg.Broadcast <| sprintf "Unknown room requested!")
                                          client.Close()
                    } |> Async.Start

                   
            finally
                printfn "Listener stopping"
                listener.Stop()        
        }
 
        Async.Start(main, token)
 
        { 
            new IDisposable 
            with member x.Dispose() = 
                    cts.Cancel() |> ignore
                    cts.Dispose()
                    agentRepo.Rooms   |> List.iter (snd >> post RoomConnMsg.Shutdown) 
        }
 
    /// Sets up a timer to broadcast the current time to the room agent
    let timer interval (ctrl : AgentRepo)  = 
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let workflow = 
            async {        
                while not <| token.IsCancellationRequested do
                    do! Async.Sleep interval
                    //ctrl.Global |> post GlobalMsg.Ping
            }

        let dispose = 
            { new IDisposable 
                with member x.Dispose() = cts.Cancel()
            }

        (workflow, dispose)    