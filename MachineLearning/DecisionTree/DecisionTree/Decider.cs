using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionTree.Data;

namespace DecisionTree
{
    public static class Decider
    {

        public static string SelectBestAxis(DecisionTreeSet set)
        {
            var baseEntropy = Entropy(set);

            //Console.WriteLine("Base entropy is {0}", baseEntropy);

            var bestInfoGain = 0.0;            

            var uniqueFeaturesByAxis = set.UniqueFeatures().GroupBy(i => i.Axis).ToList();

            string bestAxisSplit = uniqueFeaturesByAxis.First().Key;

            foreach (var axis in uniqueFeaturesByAxis)
            {                
                // calculate the total entropy based on splitting by this axis. The total entropy
                // is the sum of the entropy of each branch that would be created by this split 

                var newEntropy = EntropyForSplitBranches(set, axis.ToList());

                var infoGain = baseEntropy - newEntropy;

                if (infoGain > bestInfoGain)
                {
                    bestInfoGain = infoGain;

                    bestAxisSplit = axis.Key;
                }
            }

            return bestAxisSplit;
        }        

        private static double EntropyForSplitBranches(DecisionTreeSet set, IEnumerable<Feature> allPossibleAxisValues)
        {
            return (from possibleValue in allPossibleAxisValues 
                    select set.Split(possibleValue) into subset 
                    let prob = (float) subset.NumberOfInstances/set.NumberOfInstances 
                    select prob*Entropy(subset)).Sum();
        }

        public static double Entropy(DecisionTreeSet set)
        {
            var total = set.Instances.Count();

            var outputs = set.Instances.Select(i => i.Output).GroupBy(f => f.Value).ToList();

            var entropy = 0.0;

            foreach (var target in outputs)
            {
                var probability = (float)target.Count()/total;
                entropy -= probability*Math.Log(probability, 2);
            }

            return entropy;
        }
    }
}
