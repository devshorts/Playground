using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NaiveBayes.Data;
using NaiveBayes.Test.Fogbugz;
using NaiveBayes.Test.Fogbugz;

namespace NaiveBayes.Test
{
    [TestFixture]
    public class TestFogbugz
    {
        private static string UsersXml =  @"../../../Assets/Fogbugz/users.xml";

        [Test]
        [Ignore]
        public void TestUsers()
        {
            var userParser = new Parser();

            userParser.BuildUsers(UsersXml);

            Assert.That(userParser.Users[12], Is.EqualTo("Sam Neff"));
        }

        [Test]
        [Ignore]
        public void TestCaseText()
        {
            var samCases = @"../../../Assets/Fogbugz/sam.xml";

            var userParser = new Parser();

            var cases = userParser.GetCases(UsersXml, samCases);
        }

        [Test]
        public void TestSam()
        {            
            TestParser(@"../../../Assets/Fogbugz/sam3.xml");
        }

        [Test]
        public void TestCarlo()
        {
            TestParser(@"../../../Assets/Fogbugz/carlo.xml");
        }

        [Test]
        public void TestAnton()
        {
            TestParser(@"../../../Assets/Fogbugz/anton.xml");
        }

        [Test]
        public void TestPriya()
        {
            TestParser(@"../../../Assets/Fogbugz/priya.xml");
        }

        [Test]
        public void TestWalt()
        {
            TestParser(@"../../../Assets/Fogbugz/walt.xml");
        }

        [Test]
        public void TestDJ()
        {
            TestParser(@"../../../Assets/Fogbugz/dj.xml");
        }

        [Test]
        public void TestFaisal()
        {
            TestParser(@"../../../Assets/Fogbugz/faisal.xml");
        }

        [Test]
        public void TestKhiem()
        {
            TestParser(@"../../../Assets/Fogbugz/khiem.xml");
        }

        [Test]
        public void TestBg()
        {
            TestParser(@"../../../Assets/Fogbugz/bg.xml");
        }

        [Test]
        public void Charles()
        {
            TestParser(@"../../../Assets/Fogbugz/charles.xml");
        }

        [Test]
        public void DaveWalker()
        {
            TestParser(@"../../../Assets/Fogbugz/dwalker.xml");
        }

        [Test]
        public void Foley()
        {
            TestParser(@"../../../Assets/Fogbugz/foley.xml");
        }

        public void TestParser(string path)
        {                      
            var parser = new Parser();

            var cases = parser.GetCases(UsersXml, path);

            Func<Case, Document> caseTransform =
                i =>
                i.ToDoc(@case => @case.Area,
                        @case => @case.Title);

            #region Event Text comment 
            //@case.Events.Aggregate("", (acc, bug) => acc + " " + Regex.Replace(bug.Text, Regex.Escape("[") + "code" + Regex.Escape("]") + ".*" + Regex.Escape("[") + "/code" + Regex.Escape("]"), "")));
            #endregion

            var total = cases.Count();

            var trainingSet = cases.Take((int)(total * (3.0 / 4))).Select(caseTransform).ToList();

            var validationSet = cases.Skip((int)(total * (3.0 / 4))).Select(caseTransform).ToList();

            var trainedData = NaiveBayes.TrainBayes(trainingSet);

            var successRate = 0.0;

            foreach (var @case in validationSet)
            {
                if (NaiveBayes.Classify(@case, trainedData).Name == @case.Class.Name)
                {
                    successRate++;
                }
            }

            successRate = successRate/validationSet.Count();

            foreach (var type in trainedData.Probabilities)
            {
                Console.WriteLine(type.Class.Name);
                Console.WriteLine("--------------------");

                type.Top(10).ForEach(i => Console.WriteLine("[{1:0.00}] {0}", i.Word, i.Probability));

                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Prediction success rate is {0:0.00}%", successRate * 100);
        }        
    }
}
