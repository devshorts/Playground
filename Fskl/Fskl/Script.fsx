// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Library1.fs"
open Fskl

// Define your library scripting code here

    let private headTail list = 
        match list with 
            | h::[] -> (h, [])
            | h::t -> (h, t)
            | _ -> failwith "cannot get head and tail on empty list"

    let transpose (xs:'a list list) = 
         xs 
            |> Seq.unfold (fun (state :'a list list) -> 
                                if List.isEmpty <| List.head state then 
                                    None 
                                else
                                    let transposed = List.map headTail state |> Seq.toList                            
                                    Some(List.map fst transposed, List.map snd transposed)) 
            |> Seq.toList

    let pad (amt: int) (elem: 'a) (list: 'a seq) : 'a list = 
        if Seq.length list >= amt then 
            list |> Seq.toList
        else
            let padAmount = amt - Seq.length list
            (list |> Seq.toList) @ (List.replicate padAmount elem)

    let toCharList (str:string) = str.ToCharArray() |> Array.toList

    let words = ["Kernel"; "Microcontroller"; "Register"; "Memory"; "Operator"] |> List.map toCharList

    let max = List.map List.length words |> List.max 

    let padBy = pad max ' '

    let paddedWords = List.map padBy words

    let cols = paddedWords |> transpose

    let charListToStr list = List.fold (fun acc i -> acc + i.ToString()) "" list

    let printStrings = List.map charListToStr cols