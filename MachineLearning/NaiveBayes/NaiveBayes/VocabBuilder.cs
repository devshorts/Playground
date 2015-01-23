using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NaiveBayes.Data;

namespace NaiveBayes
{
    public static class VocabBuilder
    {
        /// <summary>
        /// Create a unique set of words in the vocab space
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public static List<string> Vocab(IEnumerable<Document> documents)
        {
            return new HashSet<string>(documents.SelectMany(doc => doc.Words)).ToList();
        }

        private static List<string> _stopWords =
            "a,able,about,across,after,all,almost,also,am,among,an,and,any,are,as,at,be,because,been,but,by,can,cannot,could,dear,did,do,does,either,else,ever,every,for,from,get,got,had,has,have,he,her,hers,him,his,how,however,i,if,in,into,is,it,its,just,least,let,like,likely,may,me,might,most,must,my,neither,no,nor,not,of,off,often,on,only,or,other,our,own,rather,said,say,says,she,should,since,so,some,than,that,the,their,them,then,there,these,they,this,tis,to,too,twas,us,wants,was,we,were,what,when,where,which,while,who,whom,why,will,with,would,yet,you,your"
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

        private static List<string> _punctuation = new List<string>{"!", ".", ",", ";", "\"", "/", "'", ":", "-", "_", "+", "?", "[", "]", "=", "(", ")", ">", "<"}; 

        private static String StripPunctuation(string input)
        {
            return _punctuation.Aggregate(input, (acc, item) => acc.Replace(item, ""));
        }

        private static String RemoveNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]*", "");
        }

        private static String StripStopWords(string input)
        {
            var word = StripPunctuation(input);

            word = RemoveNumbers(word);

            word = word.ToLowerInvariant().Trim();

            if (String.IsNullOrWhiteSpace(word))
            {
                return null;
            }

            if (string.IsNullOrEmpty(word))
            {
                return null;
            }
            
            if (!_stopWords.Contains(word))
            {
                return word;
            }

            return null;
        }
        public static Document Tokenize(string source)
        {
            return new Document
            {
                Words = source.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(StripStopWords)
                              .Where(s => !String.IsNullOrEmpty(s))
                              .ToList()
            };
        }
    }
}
