// Learn more about F# at http://fsharp.net

open System
open FloodFillTypes

let rand = new Random()

let board = { new IBoard<Earth> with
                member x.board =                       
                                  array2D    [[Land;  Land;  Water;  Land;];
                                              [Water; Land;  Water;  Land;];
                                              [Land;  Land;  Water;  Land;];
                                              [Water; Land;  Land;   Water;]]

                member x.xSize = (Array2D.length1 x.board) - 1

                member x.ySize = (Array2D.length2 x.board) - 1               
}

let masses = FloodFill.getContiguousBlocks board Land

let largestList = Seq.maxBy(Seq.length) masses

System.Console.WriteLine("Largest mass is " + (Seq.length largestList).ToString());

let p = 0;