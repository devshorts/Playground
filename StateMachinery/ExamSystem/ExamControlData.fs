namespace ExamSystem

open System
open ExamSystem
open System.Net.Sockets

[<AutoOpen>]
module ExamControlData =
    type Agent<'T> = MailboxProcessor<'T>

    type RoomId = int
    type ParticipantId = int

    type RoomConnMsg = 
        | Connect of TcpClient
        | Disconnect of TcpClient
        | Broadcast of string
        | BroadcastExcept of TcpClient * String 
        | Help of TcpClient  
        | Shutdown

    type ControlInterfaceMsg = 
        | Connect of TcpClient
        | Disconnect of TcpClient
        | Broadcast of string   
        | BroadcastTo of TcpClient * string  
        | Shutdown
        | GetRoom of RoomId * AsyncReplyChannel<StateManager.Room>
        | Advance of RoomId
        | Reverse of RoomId
        | AddParticipant of (RoomId * ParticipantId)
        | Record of RoomId
        | Reset of RoomId
        | StartPreview of RoomId        
        | StartStreaming of RoomId
        | Help of TcpClient

    type GlobalMsg = 
        | Broadcast of string


    type AgentRepo = {
        Global: Agent<GlobalMsg>;
        Rooms: (RoomId * Agent<RoomConnMsg>) list;
        Control: Agent<ControlInterfaceMsg>
    }

    
    type FailedClients = TcpClient list
    type SucceedClients = FailedClients
    
