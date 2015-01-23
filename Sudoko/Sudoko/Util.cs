using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoko
{
    
    public static class Util
    {
        /// <summary>
        /// Fisher-Yates shuffle, for funsies. Stolen from 
        /// http://stackoverflow.com/a/1262619/310196
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static bool Validate(Board board)
        {
            var totalSpaceValues = board.TotalSpaceValues;

            foreach (var location in board.AllLocations)
            {
                var used = board.UsedNumbersInSpace(location);

                if (totalSpaceValues.Except(used).Count() != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}