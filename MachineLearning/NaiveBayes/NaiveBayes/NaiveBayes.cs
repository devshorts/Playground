using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using NaiveBayes.Data;

namespace NaiveBayes
{
    public static class NaiveBayes
    {
        public static Class Classify(Document document, TrainedData trainedData)
        {
            var vocabVector = document.VocabListVector(trainedData.Vocab);

            var highestProbability = double.MinValue;

            Class classified = null;

            foreach (var @class in trainedData.Probabilities)
            {
                var probablityOfWordsInClass = vocabVector.PointwiseMultiply(@class.ProbabilityVector).Sum();

                var probablity = probablityOfWordsInClass + Math.Log(@class.ProbablityOfClass);

                if (probablity > highestProbability)
                {
                    highestProbability = probablity;

                    classified = @class.Class;
                }
            }

            return classified;
        }

        public static TrainedData TrainBayes(List<Document> trainingDocuments)
        {
            var vocab = VocabBuilder.Vocab(trainingDocuments);

            var classes = trainingDocuments.GroupBy(doc => doc.Class.Name).ToList();

            var classProbabilities = new List<ClassProbability>();

            foreach (var @class in classes)
            {
                Vector<double> countPerWordInVocabSpace = DenseVector.Create(vocab.Count, i => 1);

                countPerWordInVocabSpace = @class.Select(doc => doc.VocabListVector(vocab))
                                                 .Aggregate(countPerWordInVocabSpace, (current, docVocabVector) => current.Add(docVocabVector));

                var totalWordsFound = 2 + countPerWordInVocabSpace.Sum();

                var probablityVector = DenseVector.OfEnumerable(countPerWordInVocabSpace.Select(i => Math.Log(i/totalWordsFound)));

                // create an easy to read list of words and its probablity
                var probabilityPerWord = probablityVector.Zip(vocab, (occurence, word) => new WordProbablity { Probability = occurence, Word = word })
                                                         .ToList();

                var probabilityOfClass = 1.0 / classes.Count();

                classProbabilities.Add(new ClassProbability
                {
                    Class = @class.First().Class,
                    ProbablityOfClass = probabilityOfClass,
                    ProbablitiesList = probabilityPerWord,
                    ProbabilityVector = probablityVector
                });
            }

            return new TrainedData
            {
                Probabilities = classProbabilities,
                Vocab = vocab
            };
        } 
    }
}
