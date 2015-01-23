namespace ExamSystem 

module RoomConnections = 

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

    type private RoomAgentState = {
        Connections : TcpClient list
        AgentRepo : AgentRepo
        Inbox : Agent<RoomConnMsg>
        RoomId : int
    }

    /// Sits on the client's socket stream and broadcasts its messages
    /// to everyone else in the room
    let rec private processClientData (roomConn:Agent<RoomConnMsg>) client = 
        async{
            do! Async.SwitchToNewThread() 
            try
                for message in client |> packets do
                    match message with 
                        | HelpRequest _ -> roomConn.Post (RoomConnMsg.Help client)
                        | _ -> roomConn.Post (RoomConnMsg.BroadcastExcept (client, message))
                    
            with
                | exn -> roomConn.Post (RoomConnMsg.Disconnect client)
        }

    let private postDisconnect (inbox:Agent<RoomConnMsg>) client = inbox.Post (RoomConnMsg.Disconnect client)

    let private processRoomMsg state = function        
        | RoomConnMsg.Help client -> 
            "This is the help" |> writeStrToSocket client |> ignore
            state

        | RoomConnMsg.Connect client    ->
            state.AgentRepo.Global |> post (GlobalMsg.Broadcast <| sprintf "Client connected to room %d" state.RoomId)
                            
            monitor isConnected (postDisconnect state.Inbox) client |> Async.Start

            (state.Inbox, client) ||> processClientData |> Async.Start

            { state with Connections = client::state.Connections }

        | RoomConnMsg.Disconnect client -> 
            client.Close()
            
            { state with Connections = state.Connections |> removeTcp <| client }

        | RoomConnMsg.Broadcast msg -> 
            { state with Connections = msg |> broadcastStr state.Connections |> snd }

        | RoomConnMsg.BroadcastExcept (client, msg) ->                             
            let successFull = msg |> broadcastStr ((List.filter ((<>) client)) state.Connections) |> snd 
                                
            { state with Connections = client::successFull }

        | RoomConnMsg.Shutdown -> 
            "Shutting down" |> strToBytes |> broadcast state.Connections |> ignore
            
            List.iter closeClient state.Connections
                                
            { state with Connections = [] }

    /// An agent for a particular room
    let roomConnection agentRepo roomId = new Agent<RoomConnMsg>(fun inbox ->                
        let rec loop state =
            async {   
                //printfn "Executing room loop %d" roomId
                let! msg = inbox.Receive() 

                let originalConnectionSize = List.length state.Connections                              
                    
                let newState = processRoomMsg state msg
                 
                if originalConnectionSize <> List.length newState.Connections then
                    printfn "[room %d] total clients %d" roomId <| List.length newState.Connections

                return! loop newState
            }

        loop {  Connections = [] 
                AgentRepo = agentRepo()
                RoomId = roomId
                Inbox = inbox
                })
    
  