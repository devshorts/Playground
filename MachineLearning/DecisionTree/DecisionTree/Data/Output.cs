using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DecisionTree.Data
{
    [DataContract]
    public class Output : Feature
    {
        public Output(string value, string @class)
            : base(value, @class)
        {
        }
    }
}
