using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace DecisionTree.Data
{
    public class DecisionTreeSet
    {
        public List<Instance> Instances { get; set; } 

        public int NumberOfFeatures
        {
            get
            {
                var first = Instances.FirstOrDefault();
                if (first != null)
                {
                    return first.Features.Count;
                }
                return 0;
            }
        }

        public int NumberOfInstances
        {
            get { return Instances.Count(); }
        }

        public Boolean InstancesAreSameClass
        {
            get
            {
                return Instances.Select(i => i.Output).DistinctBy(i => i.Value).Count() == 1;
            }
        }

        public DecisionTreeSet Split(Feature feature)
        {
            return Split(feature.Axis, feature.Value);
        }

        public DecisionTreeSet Split(string axis, string value)
        {            
            return new DecisionTreeSet
                   {
                       Instances = Instances.Select(i => i.Split(axis, value))
                                            .Where(i => i.Features.Any())
                                            .ToList()
                   };
        }

        public Tree BuildTree()
        {
            if (InstancesAreSameClass || Instances.All(f => f.Features.Count() == 1))
            {
                return LeafTreeForRemainingFeatures();
            }

            var best = Decider.SelectBestAxis(this);

            return SplitByAxis(best);
        }

        private Tree SplitByAxis(string axis)
        {
            if (axis == null)
            {
                return null;
            }

            // split the set on each unique feature value where the feature is 
            // if of the right axis
            var splits = (from feature in UniqueFeatures().Where(a => a.Axis == axis)
                          select new {splitFeature = feature, set = Split(feature)}).ToList();

            var branches = new Dictionary<Feature, Tree>();

            // for each split, either recursively create a new tree
            // or split the final feature outputs into leaf trees
            foreach (var item in splits)
            {
                branches[item.splitFeature] = item.set.BuildTree();
            }

            return new Tree
                   {                       
                       Branches = branches
                   };
        }

        private Tree LeafTreeForRemainingFeatures()
        {
            if (InstancesAreSameClass)
            {
                return GroupByClass();
            }

            if (Instances.All(f => f.Features.Count() == 1))
            {
                return LeafForEachFeature();
            }

            return null;
        }

        private Tree LeafForEachFeature()
        {
            // each feature is the last item
            var branches = new Dictionary<Feature, Tree>();
                        
            foreach (var instance in Instances)
            {
                foreach (var feature in instance.Features)
                {
                    if (branches.Any(k => k.Key.Value == feature.Value))
                    {
                        continue;
                    }

                    branches[feature] = new Tree
                    {
                        Leaf = instance.Output
                    };
                }
            }
            
            return new Tree
            {
                Branches = branches
            };
        }

        private Tree GroupByClass()
        {
            var groupings = Instances.DistinctBy(i => i.Output.Value)
                                         .ToDictionary(i => i.Features.First(), j => new Tree
                                         {
                                             Leaf = j.Output
                                         });

            if (groupings.Count() > 1)
            {
                return new Tree
                {
                    Branches = groupings
                };
            }

            return new Tree
            {
                Leaf = groupings.First().Value.Leaf
            };
        }

        public IEnumerable<Feature> UniqueFeatures()
        {
            return Instances.SelectMany(f => f.Features).DistinctBy(f => f.Axis + f.Value).ToList();
        } 
    }
}


