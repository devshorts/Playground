module Game

open System
open System.Collections.Generic
    
type Position = int * int

type NeighborCount = int

type Cell = 
    | Alive of NeighborCount
    | Dead of NeighborCount

type Rule = Cell -> Cell

type CellContainer = Dictionary<Position, Cell>

type SparseBoard = {
    Size: int
    Board: CellContainer
}

let neighbors max (x, y) = 
    seq {
        yield (x + 1, y)
        yield (x - 1, y)
        yield (x, y + 1)
        yield (x, y - 1)
        yield (x + 1, y + 1)
        yield (x + 1, y - 1)
        yield (x - 1, y + 1)
        yield (x - 1, y - 1)
    } 
        |> Seq.filter (fun (x, y) -> x >= 0 && y >=0 && x < max && y < max)
        |> Seq.toList

let set value = function
    | Alive alive -> Alive value
    | Dead alive -> Dead value

let updateCell modifier = function
    | Alive alive -> Alive (modifier alive)
    | Dead alive -> Dead (modifier alive)
        
/// Gets the cell from the board.  If the cell doesn't exist
/// It means the cell is a Dead of count 0 zero
let getCell pos (board : CellContainer) = 
    if board.ContainsKey pos then board.[pos]
    else Dead 0

/// Prints the board to the screen
let print (game : SparseBoard) =
    for x in[0.. game.Size - 1] do
        for y in [0.. game.Size - 1] do
            printf "%15A " (game.Board |> getCell (x,y))
        printfn ""


/// Counts up the number of alive neighbors
let countNeighbors pos ( game : SparseBoard ) = 
    List.fold(fun acc neighbor -> 
                    // if the neighbor doesn't exist they are either dead or invalid
                    if not (game.Board.ContainsKey neighbor) then acc else
                    match game.Board.[neighbor] with  
                        | Alive count -> acc + 1
                        | Dead count -> acc) 0 (pos |> neighbors game.Size )



/// sets the positions alive count based on the count of 
/// alive neighbors
let setAliveCount pos (game : SparseBoard) =
    game.Board.[pos] <- (game.Board |> getCell pos) |> set (countNeighbors pos game)
    
/// Create a new board of size N.
/// Initializes the game board, and counts all the alive neighbors
let init n =
    let game = { Size = n; Board = new CellContainer() }

    let rand = new System.Random();
    let randomCell () = 
        if rand.Next(0, 100) > 50 then 
            Alive 0
        else 
            Dead 0

    let changed = new ResizeArray<Position * Cell>()
    for x in [0..n - 1] do
        for y in [0..n - 1] do
            match randomCell() with 
                | Alive _ -> 
                    game.Board.[(x,y)] <- Alive 0
                    changed.Add ((x,y), game.Board.[(x,y)])
                | Dead _ -> ()

    
    for (pos, changedCell) in changed do
        for neighbor in pos |> neighbors n do
            setAliveCount neighbor game

    game

// === RULES == 

let lessThan2 cell = 
    match cell with
        | Alive count when count < 2 -> Dead count
        | _ -> cell

let moreThan3 cell = 
    match cell with
        | Alive count when count > 3 -> Dead count
        | _ -> cell

let comeAlive cell = 
    match cell with
        | Dead count when count = 3 -> Alive count
        | _ -> cell

let rules : Rule list = [lessThan2; moreThan3; comeAlive]

let apply (rules : Rule list) cell = 
    List.fold (fun acc rule -> rule acc) cell rules

/// Does an iteration of the game and returns a new board
/// Does not modify the source board
let nextBoard (inputGame : SparseBoard) = 
    let game = { Size = inputGame.Size; Board = new CellContainer() }

    let changed = new ResizeArray<Position * Cell>()
    for item in inputGame.Board do
        let cell = item.Value
        let pos = item.Key
        let nextCell = apply rules cell
        if nextCell <> cell then
            changed.Add(pos, nextCell)
        match nextCell with 
            | Alive _ -> game.Board.[pos] <- nextCell
            | Dead _ -> ()

    printfn "%i changed" changed.Count

    for (changedPosition, changedCell) in changed do
        for neighbor in changedPosition |> neighbors game.Size do
            game |> setAliveCount neighbor 

        game |> setAliveCount changedPosition 

    game
                 
let rec run board n =
    if n = 0 then ()
    else run (nextBoard board) (n - 1)
             


