using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace NaiveBayes.Data
{
    public class ClassProbability
    {
        public Class Class { get; set; }

        public List<WordProbablity> ProbablitiesList { get; set; }

        /// <summary>
        /// A vector representation of a word in the vocab space
        /// </summary>
        public Vector<double> ProbabilityVector { get; set; }

        public double ProbablityOfClass { get; set; }

        public List<WordProbablity> Top(int num)
        {            
            return ProbablitiesList.OrderByDescending(i => i.Probability).Take(num).ToList();
        }

        public override string ToString()
        {
            return Class.Name;
        }
    }

    public class TrainedData
    {
        public List<WordProbablity> Top(int n, string @class)
        {
            return Probabilities.FirstOrDefault(c => c.Class.Name == @class).Top(n);
        }

        public List<ClassProbability> Probabilities { get; set; }

        public List<string> Vocab { get; set; } 
    }
}
