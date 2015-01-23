module Fskl

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

let intersperse (elem: 'a) (list: 'a seq) : 'a list = 
    let length = Seq.length list
    seq {
        for element in list do
            yield element
            yield elem
        } |> Seq.take (length * 2 - 1) |> Seq.toList


let intercalate xs xss = intersperse xs xss |> List.collect id

let strLen (str:string) = str.Length

let lines (input:string) : string list =
    input.Replace("\r\n", "\n").Split([|'\n'|]) |> Array.filter (strLen >> ((<) 0)) |> Array.toList

let private strFolder delim input = 
    (List.fold (fun acc i -> acc + i + delim) "" input).Trim()

let unlines (input:string list) : string = strFolder System.Environment.NewLine input

let words (input:string) : string list = lines (input.Replace(" ", "\n"))

let unwords (input: string list) : string = strFolder " " input