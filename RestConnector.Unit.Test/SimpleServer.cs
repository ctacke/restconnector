using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unit.Test
{
    internal class SimpleServer : DisposableBase
    {
        private readonly HttpListener m_listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> m_getMethod;
        private readonly Func<HttpListenerRequest, string> m_postMethod;
        private readonly Func<HttpListenerRequest, string> m_putMethod;
        private readonly Func<HttpListenerRequest, string> m_deleteMethod;

        public SimpleServer(
            string prefix, 
            Func<HttpListenerRequest, string> getMethod = null,
            Func<HttpListenerRequest, string> postMethod = null,
            Func<HttpListenerRequest, string> putMethod = null,
            Func<HttpListenerRequest, string> deleteMethod = null)
        {
            // TODO: at least one method is probably required (to be of any use, anyway)
            m_getMethod = getMethod;
            m_postMethod = postMethod;
            m_putMethod = putMethod;
            m_deleteMethod = deleteMethod;

            m_listener.Prefixes.Add(prefix);

            m_listener.Start();
        }

        protected override void ReleaseManagedResources()
        {
            this.Stop();
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (m_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var context = c as HttpListenerContext;
                            string result;

                            try
                            {
                                // what's the incoming verb?
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":
                                        if (m_getMethod == null) return;
                                        result = m_getMethod(context.Request);
                                        break;
                                    case "POST":
                                        if (m_postMethod == null) return;
                                        result = m_postMethod(context.Request);
                                        break;
                                    case "PUT":
                                        if (m_putMethod == null) return;
                                        result = m_putMethod(context.Request);
                                        break;
                                    case "DELETE":
                                        if (m_deleteMethod == null) return;
                                        result = m_deleteMethod(context.Request);
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("HTTP verb {0} not supported", context.Request.HttpMethod));
                                }
                                var encodedResult = Encoding.UTF8.GetBytes(result);
                                context.Response.ContentLength64 = encodedResult.Length;
                                context.Response.OutputStream.Write(encodedResult, 0, encodedResult.Length);
                            }
                            finally
                            {
                                context.Response.OutputStream.Close();
                            }
                        }, m_listener.GetContext());
                    }
                }
                catch (HttpListenerException)
                {
                    // this often happens during Disposal - just ignore it
                }
            });
        }

        public void Stop()
        {
            m_listener.Stop();
            m_listener.Close();
        }
    }
}
