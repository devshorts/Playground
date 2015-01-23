using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace GradientAscent.Tests.Parsers
{
    public class TestSetParser : ParserBase
    {
        protected override Tuple<Vector<double>, double> ReadLine(string line)
        {
            var splits = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).Select(Convert.ToDouble).ToList();

            var v = splits.Take(splits.Count() - 1).ToList();

            // constant initial weight factor at position 0
            v.Insert(0, 1.0);

            var id = splits.Last();

            return new Tuple<Vector<double>, double>(DenseVector.OfEnumerable(v), id);
        }
    }
}
