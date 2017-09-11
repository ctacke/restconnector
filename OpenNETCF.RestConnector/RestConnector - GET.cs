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
        public string GetString(string directory)
        {
            return GetString(directory, Timeout.Infinite);
        }

        public string GetString(string directory, int timeout)
        {
            return AsyncHelper.RunSync(() => GetStringAsync(directory, timeout));
        }

        public byte[] GetBytes(string directory)
        {
            return AsyncHelper.RunSync(() => GetBytesAsync(directory, Timeout.Infinite));
        }

        public byte[] GetBytes(string directory, int timeout)
        {
            return AsyncHelper.RunSync(() => GetBytesAsync(directory, timeout));
        }

        /*
        public async Task<T> GetObject<T>(string directory, int timeout)
        {
        }
        */

        public async Task<string> GetStringAsync(string directory, int timeout)
        {
            var token = new CancellationToken();
//            var task = m_client.GetStringAsync(directory);
            var task = m_client.GetAsync(directory);

            if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
            {
                // the inner await will throw if we've been cancelled
                try
                {
                    var result = await task;
                    var stream = await result.Content.ReadAsStreamAsync();
                    var rdr = new StreamReader(stream);
                    return await rdr.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    if (ThrowOnConnectionFailure) throw ex;

                    return null;
                }
            }
            else
            {
                //cancelled
                m_client.CancelPendingRequests();
                return null;
            }
        }

        public async Task<byte[]> GetBytesAsync(string directory, int timeout)
        {
            var token = new CancellationToken();
            var task = m_client.GetByteArrayAsync(directory);
            if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
            {
                // the inner await will throw if we've been cancelled
                try
                {
                    return await task;
                }
                catch (Exception ex)
                {
                    if (ThrowOnConnectionFailure) throw ex;

                    return null;
                }
            }
            else
            {
                //cancelled
                m_client.CancelPendingRequests();
                return null;
            }
        }
    }
}
