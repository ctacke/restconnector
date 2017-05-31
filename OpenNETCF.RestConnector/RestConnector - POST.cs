// -------------------------------------------------------------------------------------------------------
// LICENSE INFORMATION
//
// - This software is licensed under the MIT shared source license.
// - The "official" source code for this project is maintained at https://github.com/ctacke/restconnector
//
// Copyright (c) 2017 OpenNETCF Consulting
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
using System.Net.Http.Headers;
using System.Collections;

namespace OpenNETCF
{
    public partial class RestConnector
    {
        public string Post(string directory, string data)
        {
            return Post(directory, data, Timeout.Infinite);
        }

        public string Post(string directory, byte[] data)
        {
            return Post(directory, data, Timeout.Infinite);
        }

        public string Post(string directory, byte[] data, int timeout)
        {
            return Post(directory, data, null, timeout);
        }

        public string Post(string directory, string data, int timeout)
        {
            return Post(directory, data, null, timeout);
        }

        public string Post(string directory, XElement data)
        {
            return Post(directory, data.ToString(), MimeType.Xml, Timeout.Infinite);
        }

        public string Post(string directory, XElement data, int timeout)
        {
            return Post(directory, data.ToString(), MimeType.Xml, timeout);
        }

        public string Post(string directory, string data, string contentType, int timeout)
        {
            return AsyncHelper.RunSync(() => PostAsync(directory, data, contentType, timeout));
        }

        public string Post(string directory, byte[] data, string contentType, int timeout)
        {
            return AsyncHelper.RunSync(() => PostAsync(directory, data, contentType, timeout));
        }

        /*
        public string Post(string directory, object data, string contentType, int timeout)
        {
            return AsyncHelper.RunSync(() => PostAsync(directory, data, timeout));
        }
        */

        public async Task<string> PostAsync(string directory, string data)
        {
            return await PostAsync(directory, data, Timeout.Infinite);
        }

        public async Task<string> PostAsync(string directory, byte[] data)
        {
            return await PostAsync(directory, data, Timeout.Infinite);
        }

        public async Task<string> PostAsync(string directory, byte[] data, int timeout)
        {
            return await PostAsync(directory, data, null, timeout);
        }

        public async Task<string> PostAsync(string directory, string data, int timeout)
        {
            return await PostAsync(directory, data, null, timeout);
        }

        public async Task<string> PostAsync(string directory, XElement data)
        {
            return await PostAsync(directory, data.ToString(), MimeType.Xml, Timeout.Infinite);
        }

        public async Task<string> PostAsync(string directory, XElement data, int timeout)
        {
            return await PostAsync(directory, data.ToString(), MimeType.Xml, timeout);
        }

        public async Task<string> PostAsync(string directory, string data, string contentType, int timeout)
        {
            return await SendDataAsync("POST", directory, data, contentType, timeout);
        }

        public async Task<string> PostAsync(string directory, byte[] data, string contentType, int timeout)
        {
            return await SendDataAsync("POST", directory, data, contentType, timeout);
        }
    }
}
