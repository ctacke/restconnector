// -------------------------------------------------------------------------------------------------------
// LICENSE INFORMATION
//
// - This software is licensed under the MIT shared source license.
// - The "official" source code for this project is maintained at https://github.com/ctacke/restconnector
//
// Copyright (c) 2010 OpenNETCF Consulting
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// -------------------------------------------------------------------------------------------------------
using Output = System.Diagnostics.Debug;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenNETCF
{
    public class RestConnector : DisposableBase
    {
        private object m_syncRoot = new object();
        private HttpClientHandler m_credentialHandler;
        private HttpClient m_client;

        public RestConnector(string endpointAddress)
        {
            Validate
                .Begin()
                .ParameterIsNotNullOrWhitespace(endpointAddress, "endpointAddress")
                .Check();

            Initialize(endpointAddress);
        }

        public RestConnector(string endpointAddress, string username, string password)
        {
            Validate
                .Begin()
                .ParameterIsNotNullOrWhitespace(endpointAddress, "endpointAddress")
                .Check();

            Initialize(endpointAddress, username, password);
        }

        public RestConnector(Uri endpointAddress)
        {
            Validate
                .Begin()
                .ParameterIsNotNull(endpointAddress, "endpointAddress")
                .Check();

            Initialize(endpointAddress);
        }

        public RestConnector(Uri endpointAddress, string username, string password)
        {
            Validate
                .Begin()
                .ParameterIsNotNull(endpointAddress, "endpointAddress")
                .Check();

            Initialize(endpointAddress, username, password);
        }

        private void Initialize(string endpointAddress, string username = null, string password = null)
        {
            Validate
                .Begin()
                .ParameterIsNotNullOrWhitespace(endpointAddress, "endpointAddress")
                .Check();

            var uri = new Uri(endpointAddress);
            Initialize(uri, username, password);
        }

        private void Initialize(Uri endpointAddress, string username = null, string password = null)
        {
            Validate
                .Begin()
                .ParameterIsNotNull(endpointAddress, "endpointAddress")
                .Check();

            if (username.IsNullOrWhiteSpace())
            {
                m_client = new HttpClient();
            }
            else
            { 
                m_credentialHandler = new HttpClientHandler();
                m_credentialHandler.Credentials = new NetworkCredential(username, password);
                m_credentialHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

                m_client = new HttpClient(m_credentialHandler);
            }

            m_client.BaseAddress = endpointAddress;
        }

        protected override void ReleaseManagedResources()
        {
            base.ReleaseManagedResources();
            if (m_client != null)
            {
                m_client.Dispose();
            }
        }

        public int Port
        {
            get
            {
                if (m_client == null)
                {
                    return -1;
                }

                return m_client.BaseAddress.Port;
            }
        }

        public string EndpointAddress
        {
            get
            {
                if (m_client == null)
                {
                    return null;
                }

                return m_client.BaseAddress.ToString();
            }
        }

        public string Get(string directory)
        {
            return Get(directory, Timeout.Infinite);
        }

        public string Get(string directory, int timeout)
        {
            return AsyncHelper.RunSync(() => GetAsync(directory, timeout));
        }

        public async Task<string> GetAsync(string directory, int timeout)
        {
            var token = new CancellationToken();
            var task = m_client.GetStringAsync(directory);
            if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
            {
                // the inner await will throw if we've been cancelled
                return await task;
            }
            else
            {
                //cancelled
                m_client.CancelPendingRequests();
                return null;
            }
        }

        public byte[] GetBytes(string directory, int timeout)
        {
            return AsyncHelper.RunSync(() => GetBytesAsync(directory, timeout));
        }

        public async Task<byte[]> GetBytesAsync(string directory, int timeout)
        {
            var token = new CancellationToken();
            var task = m_client.GetByteArrayAsync(directory);
            if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
            {
                // the inner await will throw if we've been cancelled
                return await task;
            }
            else
            {
                //cancelled
                m_client.CancelPendingRequests();
                return null;
            }
        }

        private async Task<string> SendDataAsync(string method, string directory, int timeout)
        {
            return await SendDataAsync(method, directory, (string)null, null, timeout);
        }

        private async Task<string> SendDataAsync(string method, string directory, string data)
        {
            return await SendDataAsync(method, directory, data, null, Timeout.Infinite);
        }

        private async Task<string> SendDataAsync(string method, string directory, byte[] data, string contentType, int timeout)
        {
            var content = new ByteArrayContent(data);
            if (contentType != null)
            {
                content.Headers.Add("content-type", contentType);
            }
            return await SendDataAsync(method, directory, content, timeout);
        }

        private async Task<string> SendDataAsync(string method, string directory, string data, string contentType, int timeout)
        {
            var content = new StringContent(data);
            if (contentType != null)
            {
                content.Headers.Add("content-type", contentType);
            }
            return await SendDataAsync(method, directory, content, timeout);
        }

        private async Task<string> SendDataAsync(string method, string directory, HttpContent content, int timeout)
        {
            Task<HttpResponseMessage> task;

            CancellationToken token = new CancellationToken();
            switch (method.ToUpper())
            {
                case "POST":
                    task = m_client.PostAsync(directory, content);
                    if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
                    {
                        // the inner await will throw if we've been cancelled
                        return await (await task).Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //cancelled
                        m_client.CancelPendingRequests();
                        return null;
                    }
                case "PUT":
                    task = m_client.PutAsync(directory, content);
                    if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
                    {
                        // the inner await will throw if we've been cancelled
                        return await (await task).Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //cancelled
                        m_client.CancelPendingRequests();
                        return null;
                    }
                case "DELETE":
                    task = m_client.DeleteAsync(directory);
                    if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
                    {
                        // the inner await will throw if we've been cancelled
                        return await (await task).Content.ReadAsStringAsync();
                    }
                    else
                    {
                        //cancelled
                        m_client.CancelPendingRequests();
                        return null;
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        public async Task<string> PostAsync(string directory, string data, string contentType, int timeout)
        {
            return await SendDataAsync("POST", directory, data, contentType, timeout);
        }

        public async Task<string> PostAsync(string directory, byte[] data, int timeout)
        {
            return  await SendDataAsync("POST", directory, null, timeout);
        }

        public string Post(string directory, byte[] data, int timeout)
        {
            return AsyncHelper.RunSync(() => PostAsync(directory, data, timeout));
        }

        public string Post(string directory, string data, string contentType, int timeout)
        {
            return AsyncHelper.RunSync(() => PostAsync(directory, data, contentType, timeout));
        }

        public string Post(string directory, string data, int timeout)
        {
            return Post(directory, data, null, timeout);
        }

        public string Post(string directory, XElement data, int timeout)
        {
            return Post(directory, data.ToString(), "text/xml", timeout);
        }

        public string Post(string directory, XElement data)
        {
            return Post(directory, data.ToString(), "text/xml", Timeout.Infinite);
        }

        public string Post(string directory, string data)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("POST", directory, data));
        }

        public string Put(string directory, string data, string contentType, int timeout)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("PUT", directory, data, contentType, timeout));
        }

        public string Put(string directory, string data, int timeout)
        {
            return Put(directory, data, null, timeout);
        }

        public string Put(string directory, XElement data, int timeout)
        {
            return Put(directory, data.ToString(), timeout);
        }

        public string Put(string directory, XElement data)
        {
            return Put(directory, data.ToString());
        }

        public string Put(string directory, string data)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("PUT", directory, data));
        }

        public string Delete(string directory, int timeout)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("DELETE", directory, timeout));
        }

        public string Delete(string directory)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("DELETE", directory, null));
        }

        public string Delete(string directory, XElement data, int timeout)
        {
            return AsyncHelper.RunSync(() => SendDataAsync("DELETE", directory, data.ToString(), "text/xml", timeout));
        }
    }
}
