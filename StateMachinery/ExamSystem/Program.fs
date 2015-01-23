// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module ExamMain

open System
open System.IO
open ExamSystem.ExamControlData
open ExamSystem.StateManager
open ExamSystem.RoomConnections
open ExamSystem.ExamUtils
open ExamSystem.ControlInterface
open ExamSystem.GlobalConnection

let dispose (i:IDisposable) = i.Dispose()

let defaultRoomStates() = 
    let emptyRoom i = { RoomId = i; 
                        States = { 
                                    PendingStates = [DoorNotes []; Encounter []; PostEncounter []; Followup []]; 
                                    ConsumedStates = []; 
                                    CurrentState = State.Empty 
                                 } 
                        RecorderStatus = NoStatus
                      }

    List.init 20 emptyRoom

let initializeAgentRepos() = 
    let agentRepoRef = 
                        ref { 
                            Global = new Agent<GlobalMsg>(fun _ -> async{return ()});
                            Control = new Agent<ControlInterfaceMsg>(fun _ -> async{return ()}); 
                            Rooms = (0, new Agent<RoomConnMsg>(fun _ -> async{return ()}))::[]
                        }

    let agentRepo() = !agentRepoRef

    let roomAgents = [for roomId in [1..5] -> (roomId, roomConnection agentRepo roomId)]

    let controlInterfaceAgent = controlInterface agentRepo (defaultRoomStates())

    let globalAgent = globalAgent agentRepo

    agentRepoRef := {
        Global  = globalAgent;
        Control = controlInterfaceAgent
        Rooms   = roomAgents;
    }

    !agentRepoRef

[<EntryPoint>]
let main argv =    
    
    let agentRepos = initializeAgentRepos()
    
    agentRepos.Rooms |> List.iter startRoom 

    use roomlistener = listenForRoomConnections agentRepos       

    use controlListener = listenForControlConnections agentRepos
    
    let (timer, disposable) = timer 1000 agentRepos

    timer |> Async.Start |> ignore

    printfn "press any key to stop..."
    
    async {
        while true do
            do! Async.Sleep 1000

            let room = agentRepos.Control.PostAndReply (fun chan -> ControlInterfaceMsg.GetRoom (1, chan))

            printfn "%A" room        
    } |> ignore //Async.Start 

    Console.ReadKey() |> ignore

    dispose disposable
    
    0
