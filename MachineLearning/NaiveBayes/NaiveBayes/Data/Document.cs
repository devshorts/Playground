using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NaiveBayes.Data
{
    public class Document
    {
        public List<String> Words { get; set; }

        public Class Class { get; set; }

        /// <summary>
        /// Create a vector representing the occurrence of words in the target vocab list
        /// 
        /// For example, a vocab list of ["hey", "you", "are", "great"]
        /// and a document with the tokens "Hey whats up are you here?"
        /// would generate a vector of
        /// 
        /// [1 1 1 0]
        /// 
        /// Indicating that "hey", "you", and "are" exist
        /// </summary>
        /// <param name="vocablList"></param>
        /// <returns></returns>
        public DenseVector VocabListVector(IEnumerable<string> vocablList)
        {
            var dict = new Dictionary<string, double>();
            foreach (var word in Words)
            {
                if (!dict.ContainsKey(word))
                {
                    dict[word] = 1;
                }
                else
                {
                    dict[word]++;
                }
            }

            Func<string, double> countForWord = s =>
                {
                    double count;
                    if (!dict.TryGetValue(s, out count))
                    {
                        return 0;
                    }
                    return count;
                };

            return DenseVector.OfEnumerable(vocablList.Select(countForWord));
        }  
    }
}
