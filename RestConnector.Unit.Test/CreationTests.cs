using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenNETCF;

namespace Unit.Test
{
    [TestClass]
    public class CreationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void CreateNullStringAddress()
        {
            var rc = new RestConnector((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void CreateNullUriAddress()
        {
            var rc = new RestConnector((Uri)null);
        }

        [TestMethod]
        public void CreateHttpNoCredsWithString()
        {
            var address = "http://www.opennetcf.com/";
            var rc = new RestConnector(address);
            Assert.IsNotNull(rc);
            Assert.IsTrue(rc.EndpointAddress == address);
            Assert.AreEqual(80, rc.Port);
        }
    }
}
