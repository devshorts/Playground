using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DecisionTree.Data;
using SampleDataParsers.Lenses;

namespace SampleDataParsers.Car
{
    public class Car : LineReader
    {
        protected override Instance ParseLine(string line)
        {
            var splits = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var buying = splits[0];
            var maintence = splits[1];
            var doors = splits[2];
            var people = splits[3];
            var lugBoot = splits[4];
            var safety = splits[5];

            return new Instance
            {
                Output = new Output(splits[6], "car acceptability"),
                Features = new List<Feature>
                                  {
                                      new Feature(buying, "buying"),
                                      new Feature(maintence, "maintence"),
                                      new Feature(doors, "doors"),
                                      new Feature(people, "people"),
                                      new Feature(lugBoot, "lugboot"),
                                      new Feature(safety, "safety")
                                  }
            };
        }
    }
}
