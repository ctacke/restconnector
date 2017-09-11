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
        public void GetOther()
        {
            var uri = "http://localhost:3000/";

            using (var rc = new RestConnector(uri))
            {
                Assert.IsNotNull(rc);
                rc.Accept.ContentType.Add(MimeType.Json);
                var result = rc.GetString("/api/drivers");

                Assert.IsNotNull(result);
            }

        }

        [TestMethod]
        public void GetJson()
        {
            var uri = "http://localhost:8080/";
            var response = "{\"name\":\"value\"}";

            using (var svr = new SimpleServer(uri, (request) =>
            {
                return response;
            })
            )
            {
                svr.Start();

                using (var rc = new RestConnector(uri))
                {
                    Assert.IsNotNull(rc);
                    var result = rc.GetString("/");

                    Assert.AreEqual(response, result);
                }
            }
        }

        [TestMethod]
        public void GetNoCreds()
        {
            var uri = "http://localhost:8080/";
            var response = "Hello!";

            using (var svr = new SimpleServer(uri, (request) =>
            {
                return response;
            })
            )
            {
                svr.Start();

                using (var rc = new RestConnector(uri))
                {
                    Assert.IsNotNull(rc);
                    var result = rc.GetString("/");

                    Assert.AreEqual(response, result);
                }
            }
        }

        [TestMethod]
        public void GetNoCredsTimeout()
        {
            using (var rc = new RestConnector("http://www.opennetcf.com"))
            {
                Assert.IsNotNull(rc);
                var result = rc.GetString("/", 1);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void GetInvalidDomain1()
        {
            using (var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com"))
            {
                Assert.IsNotNull(rc);
                var result = rc.GetString("/");
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void GetInvalidDomain2()
        {
            using (var rc = new RestConnector("http://www.ihopethisisanonexistentdomain.com"))
            {
                rc.ThrowOnConnectionFailure = true;
                Assert.IsNotNull(rc);
                var result = rc.GetString("/");
            }
        }
    }
}
