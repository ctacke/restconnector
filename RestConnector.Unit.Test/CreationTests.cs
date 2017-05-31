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
            using (var rc = new RestConnector(address))
            {
                Assert.IsNotNull(rc);
                Assert.IsTrue(rc.EndpointAddress == address);
                Assert.AreEqual(80, rc.Port);
            }
        }

        [TestMethod]
        public void CreateWithAcceptContentType1()
        {
            var address = "http://www.opennetcf.com/";
            using (var rc = new RestConnector(address))
            {
                Assert.IsNotNull(rc);
                rc.Accept.ContentType.Add(MimeType.Json);
            }
        }

        [TestMethod]
        public void CreateWithAcceptContentType2()
        {
            var address = "http://www.opennetcf.com/";
            using(var rc = new RestConnector(address))
            {
                Assert.IsNotNull(rc);
                rc.Accept.ContentType.Add(MimeType.Json, MimeType.Xml);
            }
        }

        [TestMethod]
        public void CreateWithAcceptEncoding1()
        {
            var address = "http://www.opennetcf.com/";
            using (var rc = new RestConnector(address))
            {
                Assert.IsNotNull(rc);
                rc.Accept.Encoding.Add(ContentEncoding.Gzip);
            }
        }
    }
}
