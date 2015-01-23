using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace GradientAscent.Tests.Parsers
{
    public class ParsedData
    {
        public Matrix<double> Data { get; set; }
        public Vector<double> ClassId { get; set; } 
    }

    public abstract class ParserBase
    {
        public ParsedData Parse(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var rows = new List<Vector<double>>();
            var ids = new List<double>();
            var count = 0;
            using (var f = new StreamReader(path))
            {
                while (!f.EndOfStream && count < 5)
                {
                    var read = ReadLine(f.ReadLine());

                    rows.Add(read.Item1);
                    ids.Add(read.Item2);

                    count++;
                }

            }

            return new ParsedData
                   {
                       Data = DenseMatrix.OfRows(rows.Count, rows.First().Count, rows),
                       ClassId = DenseVector.OfEnumerable(ids)
                   };
        }

        protected abstract Tuple<Vector<double>, double> ReadLine(string line);
    }
}
