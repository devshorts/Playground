using System;

namespace NaiveBayes.Data
{
    public class WordProbablity
    {
        public string Word { get; set; }

        public double Probability { get; set; }

        static public implicit operator double(WordProbablity value)
        {
            // Note that because RomanNumeral is declared as a struct, 
            // calling new on the struct merely calls the constructor 
            // rather than allocating an object on the heap:
            return value.Probability;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Word, Probability);
        }
    }
}
