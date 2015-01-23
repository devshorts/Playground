Sudoku solver
------

Solves sudoko with constraint based backtracking.  Here is the result of solving [Peter Norvig's](http://norvig.com/sudoku.html) 95 [hard sudoko](http://norvig.com/top95.txt) challenges

```
Succeeded in 72 ms
Succeeded in 154 ms
Succeeded in 163 ms
Succeeded in 8720 ms
Succeeded in 2234 ms
Succeeded in 18160 ms
Succeeded in 7919 ms
Succeeded in 86 ms
Succeeded in 6435 ms
Succeeded in 9515 ms
Succeeded in 1405 ms
Succeeded in 113 ms
Succeeded in 175 ms
Succeeded in 2359 ms
Succeeded in 259 ms
Succeeded in 1370 ms
Succeeded in 81 ms
Succeeded in 6814 ms
Succeeded in 6 ms
Succeeded in 118 ms
Succeeded in 12125 ms
Succeeded in 17 ms
Succeeded in 210 ms
Succeeded in 164 ms
Succeeded in 83 ms
Succeeded in 658 ms
Succeeded in 616 ms
Succeeded in 17439 ms
Succeeded in 48 ms
Succeeded in 292 ms
Succeeded in 129 ms
Succeeded in 47 ms
Succeeded in 15 ms
Succeeded in 9890 ms
Succeeded in 644 ms
Succeeded in 143 ms
Succeeded in 425 ms
Succeeded in 134 ms
Succeeded in 848 ms
Succeeded in 41 ms
Succeeded in 2396 ms
Succeeded in 1214 ms
Succeeded in 574 ms
Succeeded in 75 ms
Succeeded in 160 ms
Succeeded in 8759 ms
Succeeded in 411 ms
Succeeded in 139 ms
Succeeded in 1088 ms
Succeeded in 252 ms
Succeeded in 51 ms
Succeeded in 45 ms
Succeeded in 16 ms
Succeeded in 38 ms
Succeeded in 117 ms
Succeeded in 37 ms
Succeeded in 273 ms
Succeeded in 167 ms
Succeeded in 106 ms
Succeeded in 209 ms
Succeeded in 410 ms
Succeeded in 155 ms
Succeeded in 24 ms
Succeeded in 640 ms
Succeeded in 2 ms
Succeeded in 171 ms
Succeeded in 292 ms
Succeeded in 441 ms
Succeeded in 283 ms
Succeeded in 280 ms
Succeeded in 7822 ms
Succeeded in 24 ms
Succeeded in 572 ms
Succeeded in 353 ms
Succeeded in 262 ms
Succeeded in 267 ms
Succeeded in 127 ms
Succeeded in 34 ms
Succeeded in 820 ms
Succeeded in 57 ms
Succeeded in 194 ms
Succeeded in 191 ms
Succeeded in 293 ms
Succeeded in 122 ms
Succeeded in 59 ms
Succeeded in 17 ms
Succeeded in 223 ms
Succeeded in 5 ms
Succeeded in 269 ms
Succeeded in 50 ms
Succeeded in 31 ms
Succeeded in 12 ms
Succeeded in 106 ms
Succeeded in 41 ms
```

Not so good, but at least it finishes!  :)

The code would be a whole lot faster if I started with all possibile availables and then eliminated, instead of re-calculating the possible space on each run. But, I originally solved it in a brute force manner and added in the constraint based backtracking later, and really didn't feel like refactoring the whole thing.