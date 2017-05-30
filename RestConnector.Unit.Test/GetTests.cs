using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenNETCF;

namespace Unit.Test
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void GetNoCreds()
        {
            var rc = new RestConnector("http://www.opennetcf.com");
            Assert.IsNotNull(rc);
            var result = rc.Get("/");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetNoCreds2()
        {
            var rc = new RestConnector("http://www.opennetcf.com");
            Assert.IsNotNull(rc);
            var result = rc.Get("/", 1);
            Assert.IsNull(result);
        }
    }
}
