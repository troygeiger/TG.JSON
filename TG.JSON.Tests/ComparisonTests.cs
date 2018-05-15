using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.JSON.Tests
{
    [TestClass]
    public class ComparisonTests
    {

        [TestMethod]
        public void CompareBoolTest()
        {
            JsonBoolean a = true;
            JsonBoolean b = true;
            Assert.AreEqual(a, b);
            
            b = false;

            Assert.AreNotEqual(a, b);
        }


    }
}
