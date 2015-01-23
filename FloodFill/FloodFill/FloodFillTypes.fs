module FloodFillTypes


type Board<'T> = 'T[,]

type X = int

type Y = int

type Position = X * Y

type Earth = 
    | Land
    | Water

type IBoard<'T> = 
    abstract board : Board<'T>    
    abstract xSize : int
    abstract ySize : int    