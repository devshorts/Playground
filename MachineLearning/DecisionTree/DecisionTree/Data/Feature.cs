using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DecisionTree.Data
{
    [DataContract]
    public class Feature
    {
        protected bool Equals(Feature other)
        {
            return string.Equals(Value, other.Value) && string.Equals(Axis, other.Axis);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0)*397) ^ (Axis != null ? Axis.GetHashCode() : 0);
            }
        }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Axis { get; set; }

        public Feature(string value, string axis)
        {
            Value = value;
            Axis = axis;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Feature) obj);
        }


        public override string ToString()
        {
            return String.Format("{0}: {1}", Axis, Value);
        }
    }
}
