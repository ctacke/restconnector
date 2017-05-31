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
    public class Accept
    {
        internal event EventHandler Changed;

        public ValueCollection ContentType { get; private set; }
        public ValueCollection Encoding { get; private set; }

        internal Accept()
        {
            ContentType = new ValueCollection();
            ContentType.Changed += OnInternalChange;
            Encoding = new ValueCollection();
            Encoding.Changed += OnInternalChange;
        }

        private void OnInternalChange(object sender, EventArgs a)
        {
            Changed.Fire(this);
        }

        public class ValueCollection : IEnumerable<string>
        {
            internal event EventHandler Changed;

            private List<string> m_values = new List<string>();

            internal ValueCollection()
            {
            }

            public void Add(params string[] values)
            {
                Validate
                    .Begin()
                    .ParameterIsNotNull(values, "values")
                    .Check();

                if (values.Length == 0) return;

                lock (m_values)
                {
                    foreach (var value in values)
                    {
                        if (m_values.Contains(value, StringComparer.OrdinalIgnoreCase)) return;
                        m_values.Add(value);
                    }

                    Changed.Fire(this);
                }
            }

            public void Add(string value)
            {
                Validate
                    .Begin()
                    .ParameterIsNotNullOrWhitespace(value, "value")
                    .Check();

                lock (m_values)
                {
                    if (m_values.Contains(value, StringComparer.OrdinalIgnoreCase)) return;
                    m_values.Add(value);
                    Changed.Fire(this);
                }
            }

            public void Clear()
            {
                lock (m_values)
                {
                    m_values.Clear();
                    Changed.Fire(this);
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                lock (m_values)
                {
                    return m_values.GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
