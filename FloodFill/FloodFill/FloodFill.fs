module FloodFill

open FloodFillTypes
open Positions


let neighbors (x,y) (canvas:IBoard<'T>) = 
    seq { 
            for move in [moveUp;moveDown;moveLeft;moveRight] do
            let x',y' = move (x,y)
            if not (offBoard (x',y') canvas) then
                let neighbor = canvas.board.[x',y']
                let current = canvas.board.[x,y]
                if current = neighbor then 
                    yield x',y'                 
        } |> Seq.toList


(*
    Returns a list of points representing a contigious block 
    of the type that the point was at. 
*)

let floodFillArea (point:Position) (canvas:IBoard<'T>) (visited:bool[,]) = 
    let nodes = seq { for x in [0..canvas.xSize] do for y in [0..canvas.ySize] -> (x,y) }
    
    let emitted = Array2D.map (fun _ -> false) canvas.board

    let wasVisited (x,y) = visited.[x,y]
    let wasEmitted (x,y) = emitted.[x,y]

    let rec fill (x,y)  = 
        seq{
            if not visited.[x,y] then                
                visited.[x,y] <- true
                let validNeighbors = (neighbors (x,y) canvas)
                if not (List.isEmpty validNeighbors) then
                    let s =  validNeighbors 
                    if not (wasEmitted (x,y)) then 
                        emitted.[x,y] <- true
                        yield (x,y)
                    
                    yield! Seq.collect fill validNeighbors
                else 
                    // one piece is alwayas a contigious block
                    yield (x,y)
        }

    fill point |> Seq.toList    


(*
    Finds all contiguous blocks of the specified type
    and returns a list of lists (each list is the points for a specific
    block)
*)
    
let getContiguousBlocks (canvas:IBoard<'T>) target =    
   seq{
        let visited = Array2D.map (fun _ -> false) canvas.board

        for x in [0..canvas.xSize] do
            for y in [0..canvas.ySize] do
                let fill = floodFillArea (x,y) canvas visited
                match fill with
                    | h::_ -> if (elemAt canvas.board h) = target then yield fill        
                    | [] -> ()            
    } |> Seq.toList