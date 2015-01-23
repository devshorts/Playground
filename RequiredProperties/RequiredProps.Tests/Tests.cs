using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequiredProperties;

namespace RequiredProps.Tests
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestAttributes()
        {
            var attributes = AttributesCache.GetAttributes(typeof (TestReq));

            CollectionAssert.AreEqual(attributes, new[]{"Foo", "Bar"});
        }
    }

    public class RequiredAttribute : Attribute{}

    public class TestReq
    {
        [Required]
        public String Foo { get; set; }

        [Required]
        public String Bar { get; set; }

        public String Not { get; set; }
    }
}
