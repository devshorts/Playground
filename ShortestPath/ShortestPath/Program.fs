// Learn more about F# at http://fsharp.net

exception ElementException of string

type IBoard<'T> = 
    abstract canvas:'T[,]
    abstract xSize:int
    abstract ySize:int
    abstract canMove:int*int -> bool

type BoardElement = 
    | Open of int
    | Closed 
    | Target    

let up (x,y) = (x, y + 1)
let down (x,y) = (x, y - 1)
let right (x,y) = (x + 1, y)
let left (x,y) = (x - 1, y)
let upLeft = up >> left
let upRight = up >> right
let downLeft = down >> left
let downRight = down >> right

let isValid (x,y) (board:IBoard<'T>) = x >= 0 && y >= 0 && x < board.xSize && y < board.ySize

let neighbors (x,y) (board:IBoard<'T>) canMove = 
    seq{
        for move in [up; down; left; right; upLeft; upRight; downLeft; downRight; ] do
            let target = move (x,y)
            if (isValid target board) && (canMove target) then
                yield target
        } |> Seq.toList

let estimatedDist (x,y) (x',y') = 
    let xDelt = x' - x
    let yDelt = y' - y
    sqrt ((float)((pown xDelt 2) + (pown yDelt 2)))


let (@@) (board:IBoard<'T>) (x,y)  = Array2D.get board.canvas x y 

let dijsktra board  (r,c)  = 
    (float)
        (match board @@ (r,c) with 
            | Open weight -> weight
            | Target -> -(int)infinity 
            | Closed -> raise (ElementException("Can't go to a closed element")))

let astar board start (r,c) = estimatedDist start (r, c) + (dijsktra board (r,c))

let path (start:int*int) (board:IBoard<'T>) weightChooser target =

    let visited = Array2D.map(fun _ -> false) board.canvas
     
    let rec path' (current:int*int) = 
        seq{            
            let x, y = current

            let isVisited (r,c) = visited.[r,c]
            let visit (r, c) = visited.[r,c] <- true 

            if not (isVisited current) then
                visit current

                let canMove = fun (x,y) -> board.canMove (x,y) && not (visited.[x,y])

                let neighbors = List.filter(isVisited >> not) (neighbors current board canMove)

                if not (List.isEmpty neighbors) then                    
                    let nearestNeighbor = List.minBy(weightChooser) neighbors

                    if not ((board @@ nearestNeighbor) = target) then               
                        yield nearestNeighbor
                        yield! path' nearestNeighbor 
                    else
                        yield nearestNeighbor
        }
    path' start |> Seq.toList

let board = { 
    new IBoard<BoardElement> with
        member x.canvas = array2D [[Open(1); Open(5);     Open(1);];
                                   [Open(4); Closed;      Open(1);];
                                   [Open(1); Open(1);     Target;]]
                                    
        member x.xSize = Array2D.length1 x.canvas
        member x.ySize = Array2D.length2 x.canvas
        member x.canMove (row,col) = not (x @@ (row,col) = Closed)
    }

let dijsktraWeights pos = dijsktra board pos
let astarWeights pos = astar board (2,2) pos

let dijPath = path (0,0) board dijsktraWeights Target
let astarPath = path (0,0) board astarWeights Target

let x = 0;