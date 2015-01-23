using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DecisionTree.Data;

namespace SampleDataParsers
{
    public abstract class LineReader : IParser
    {
        public DecisionTreeSet Parse(string file)
        {
            var set = new DecisionTreeSet();

            set.Instances = new List<Instance>();

            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    set.Instances.Add(ParseLine(line));
                }
            }

            return set;
        }

        protected abstract Instance ParseLine(string line);
    }
}
