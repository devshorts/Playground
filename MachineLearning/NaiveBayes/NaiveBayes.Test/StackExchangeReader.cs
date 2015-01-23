using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaiveBayes.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NaiveBayes.Test
{    
    public static class StackExchangeReader
    {
        public static List<Document> Parse(string source, string @class)
        {
            var serialized = JsonConvert.DeserializeObject<JObject>(source);

            return (serialized["items"] as JArray).Select(token => new Document
                                                                   {
                                                                       Words = VocabBuilder.Tokenize(token["title"].ToString()).Words,
                                                                       Class = new Class {Name = @class}
                                                                   }).ToList();
        } 
    }
}
