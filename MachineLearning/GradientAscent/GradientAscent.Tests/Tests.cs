using GradientAscent.Tests.Parsers;
using NUnit.Framework;

namespace GradientAscent.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestClassData()
        {
            var data = new TestSetParser().Parse("Assets/testSet.txt");
        }

        [Test]
        public void GradientAscentTest()
        {
            var data = new TestSetParser().Parse("Assets/testSet.txt");

            var weights = GradientRunner.GetWeights(data.Data, data.ClassId);

        }
    }
}
