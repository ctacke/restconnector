using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenNETCF;
using System.Net.Http;

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

        [TestMethod]
        public void GetInvalidDomain1()
        {
            var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com");
            Assert.IsNotNull(rc);
            var result = rc.Get("/");
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void GetInvalidDomain2()
        {
            var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com");
            rc.ThrowOnConnectionFailure = true;
            Assert.IsNotNull(rc);
            var result = rc.Get("/");
        }
    }
}
