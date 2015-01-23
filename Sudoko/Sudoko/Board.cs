using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using MoreLinq;

namespace Sudoko
{
    [Serializable]
    public class Board
    {
        private readonly int?[,] _board;

        private readonly List<Location> _emptySpaces = new List<Location>();

        public static Board Load(List<char> representation, int n)
        {

            if (n > 3)
            {
                throw new Exception("Parse only works for n <=3");
            }

            var board = new int?[n * n, n * n];

            var count = 0;
            for (int i = 0; i < n * n; i++)
            {
                for (int j = 0; j < n * n; j++)
                {
                    if (representation[count] != '.')
                    {
                        board[i, j] = Int32.Parse(representation[count].ToString());
                    }

                    count++;
                }
            }

            return new Board(n, board);
        }

        public Board(int n, int?[,] board)
        {
            N = n;

            _board = board;

            ForAllElements(location =>
            {
                if (_board[location.X, location.Y] == null)
                {
                    _emptySpaces.Add(location);
                }
            });

            TotalSpaceValues = Enumerable.Range(1, N * N).ToList();
        }

        public Board(int n) : this(n, new int?[n * n, n * n])
        {

        }

        public List<int> TotalSpaceValues { get; private set; } 

        private void ForAllElements(Action<Location> action)
        {
            for (int i = 0; i < N * N; i++)
            {
                for (int j = 0; j < N * N; j++)
                {
                    action(new Location(i, j));
                }
            }
        }

        public Board Snapshot()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (Board)formatter.Deserialize(ms);
            }
        }

        public int N { get; private set; }

        public int? Get(int x, int y)
        {
            if (x > _board.Length || y > _board.Length)
            {
                throw new Exception("invalid position");
            }

            return _board[x, y];
        }

        public void Set(Location location, int value)
        {
            _board[location.X, location.Y] = value;

            for (int i = 0; i < _emptySpaces.Count; i++)
            {
                if (_emptySpaces[i].X == location.X && _emptySpaces[i].Y == location.Y)
                {
                    _emptySpaces.RemoveAt(i);
                    return;
                }
            }
        }

        public IEnumerable<Location> AllLocations
        {
            get
            {
                for (int i = 0; i < N * N; i++)
                {
                    for (int j = 0; j < N * N; j++)
                    {
                        yield return new Location(i, j);
                    }
                }
            }
        }


        public IEnumerable<int> UsedNumbersInSpace(Location location)
        {
            int x = location.X;
            int y = location.Y;

            foreach (var item in GetCol(x, y))
            {
                if (item.HasValue)
                {
                    yield return item.Value;
                }
            }

            foreach (var item in GetRow(x, y))
            {
                if (item.HasValue)
                {
                    yield return item.Value;
                }
            }

            foreach (var item in GetSquare(x, y))
            {
                if (item.HasValue)
                {
                    yield return item.Value;
                }
            }
        }

        private IEnumerable<int?> GetRow(int x, int y)
        {
            for (int i = 0; i < N * N; i++)
            {
                yield return Get(i, y);
            }
        }

        private IEnumerable<int?> GetCol(int x, int y)
        {
            for (int i = 0; i < N * N; i++)
            {
                yield return Get(x, i);
            }
        }

        private IEnumerable<int?> GetSquare(int x, int y)
        {
            int xStart = x - (x % N);
            int yStart = y - (y % N);

            for (int i = xStart; i < xStart + N; i++)
            {
                for (int j = yStart; j < yStart + N; j++)
                {
                    yield return Get(i, j);
                }
            }
        }


        public Location NextEmpty()
        {
            if (_emptySpaces.Count == 0)
            {
                return null;
            }

            var possibles = new Dictionary<Location, List<int>>();

            foreach (var emptySpace in _emptySpaces)
            {
                possibles[emptySpace] = TotalSpaceValues.Except(UsedNumbersInSpace(emptySpace)).ToList();

                if (possibles[emptySpace].Count == 1)
                {
                    Set(emptySpace, possibles[emptySpace].First());

                    return NextEmpty();
                }
            }
//
//            foreach (var possible in possibles)
//            {
//                if (possible.Value.Count == 1)
//                {
//                    Set(possible.Key, possible.Value.First());
//
//                    return NextEmpty();
//                }
//            }

            return possibles.MinBy(kvp => kvp.Value.Count()).Key;
        }

        public void Print()
        {
            for (int i = 0; i < N * N; i++)
            {
                for (int j = 0; j < N * N; j++)
                {
                    Console.Out.Write("{0, -4}", Get(i, j));
                }

                Console.Out.WriteLine();
            }
        }
    }
}