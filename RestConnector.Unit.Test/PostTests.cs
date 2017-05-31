using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenNETCF;
using System.Net.Http;
using System.IO;

namespace Unit.Test
{
    [TestClass]
    public class PostTests
    {
        [TestMethod]
        public void PostNoCreds()
        {
            var uri = "http://localhost:8080/";
            var payload = "Test Payload";
            var response = "Test Response";

            using (var svr = new SimpleServer(uri, postMethod: (request) =>
            {
                using (var reader = new StreamReader(request.InputStream))
                {
                    var incoming = reader.ReadToEnd();
                    Assert.AreEqual(incoming, payload);
                }
                return response;
            })
            )
            {
                svr.Start();

                using (var rc = new RestConnector(uri))
                {
                    Assert.IsNotNull(rc);
                    var result = rc.Post("/", payload);

                    Assert.AreEqual(response, result);
                }
            }
        }

        [TestMethod]
        public void PostNoCredsTimeout()
        {
            using (var rc = new RestConnector("http://www.opennetcf.com"))
            {
                Assert.IsNotNull(rc);
                var result = rc.Post("/", "test", 1);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void PostInvalidDomain1()
        {
            using (var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com"))
            {
                Assert.IsNotNull(rc);
                var result = rc.Post("/", "test");
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void PostInvalidDomain2()
        {
            using (var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com"))
            {
                rc.ThrowOnConnectionFailure = true;
                Assert.IsNotNull(rc);
                var result = rc.Post("/", "test");
            }
        }
    }
}
