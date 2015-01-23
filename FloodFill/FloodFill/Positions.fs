module Positions

open FloodFillTypes

(* 
    Helper methods to move the position around
*)
                                          
let moveRight (x,y) = (x + 1, y)
    
let moveLeft (x,y) = (x - 1, y)
    
let moveUp (x,y) =  (x, y + 1)
    
let moveDown (x,y) = (x, y - 1)

let moveDiagUpLeft (x,y) = (x - 1, y + 1)

let moveDiagUpRight (x,y) = (x + 1, y + 1)

let moveDiagDownLeft (x,y) = (x - 1, y - 1)

let moveDiagDownright (x,y) = (x + 1, y - 1)

(*
    Size helpers
*)

let offBoard (x,y) (canvas:IBoard<'T>) = x < 0 || y < 0 || x > canvas.xSize || y > canvas.ySize

(*
    Alias to push elements onto a list
*)

let markPosition position previousSpots = position::previousSpots


(*
    Get element of board at position
*)

let elemAt board (x, y) = Array2D.get board x y


(*
    Determines if the position on the board equals the target
*)

let positionOnTarget position canvas target = 
    if offBoard position canvas then 
        false
    else        
        (elemAt canvas.board position) = target

(*
    Alias to find if we already processed a position
*)

let positionExists position list = 
    List.exists(fun pos -> pos = position) list

