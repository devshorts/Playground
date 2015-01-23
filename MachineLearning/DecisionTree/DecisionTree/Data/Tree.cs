using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MoreLinq;

namespace DecisionTree.Data
{    
    [DataContract]
    [KnownType(typeof(Feature))]
    [KnownType(typeof(Output))]
    [KnownType(typeof(Tree))]
    public class Tree
    {
        [DataMember]
        public Output Leaf { get; set; }

        [DataMember]
        public Dictionary<Feature, Tree> Branches { get; set; }

        public void DisplayTree(int tab = 0)
        {
            Action tabWriter = () => Enumerable.Range(0, tab).ForEach(i => Console.Write("\t"));

            if (Branches != null)
            {
                foreach (var feature in Branches)
                {
                    tabWriter();

                    Console.WriteLine(feature.Key);

                    feature.Value.DisplayTree(tab + 1);
                }
            }
            else
            {
                tabWriter();
                Console.WriteLine(Leaf);
            }
        }

        public static Output ProcessInstance(Tree tree, Instance i)
        {
            if (tree.Leaf != null)
            {
                return tree.Leaf;
            }

            return ProcessInstance(tree.TreeForInstance(i), i);
        }

        private Tree TreeForFeature(Feature feature)
        {
            Tree found;
            if (Branches.TryGetValue(feature, out found))
            {
                return found;
            }
            return null;
        }

        private Tree TreeForInstance(Instance instance)
        {
            var tree = instance.Features.Select(TreeForFeature).FirstOrDefault(f => f != null);

            return tree;
        }
    }
}
