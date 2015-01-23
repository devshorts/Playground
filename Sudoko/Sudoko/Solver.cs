using System.Collections.Generic;
using System.Linq;

namespace Sudoko
{
    public class Solver
    {
        public static Board SolveSingleThreaded(Board b)
        {
            var nextOpen = b.NextEmpty();

            if (nextOpen == null)
            {
                return b;
            }

            var taken = b.UsedNumbersInSpace(nextOpen);

            var available = b.TotalSpaceValues
                             .Except(taken)
                             .ToList();
                             //.Shuffle();

            if (available.Count == 0)
            {
                return null;
            }

            foreach (var possible in available)
            {
                var newBoard = b.Snapshot();

                newBoard.Set(nextOpen, possible);

                var next = SolveSingleThreaded(newBoard);

                if (next != null)
                {
                    return next;
                }
            }

            return null;
        }
    }
}