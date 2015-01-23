Game of life
====

This is an F# implementation of [Conways game of life](http://en.wikipedia.org/wiki/Conway's_Game_of_Life). The rules are simple:

> 1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
> 2. Any live cell with two or three live neighbours lives on to the next generation.
> 3. Any live cell with more than three live neighbours dies, as if by overcrowding.
> 4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

The easy way to solve this is to brute force the board. On each iteration you run through the rules, count up each neighbor, and create a new board from it.

This implementation uses a sparse board (implemented as a dictionary) to only keep track of cells that are living (or were touched by modified cells).  The advantage here is that you only need to iterate over cells that have been touched recently.  As things die off, they get removed from the dictionary, and as things come to life they come back to the dictionary.   

You still need to count all the neighbors, but ONLY of changed items!  As you count the items, if the neighbor didn't exist in the dictionary you can add it, and now it will join the processing pool.  If a node isn't changed, and none of its neighbors are changed, then there is no reason to run through it.

For example, running the game 50 times on a board of 250,000 elements took just under 3 minutes for me. 50 times on a board of 1 million elements took 30 minutes.