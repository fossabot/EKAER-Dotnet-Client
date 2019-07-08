// Copyright (c) 2019 Péter Németh
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using EKAER.Schema.Management;
using EKAER.Utils;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EKAER.Client
{
    public class BaseClient
    {
        protected readonly Uri baseUrl = null;
        protected readonly string username;
        protected readonly HexString passwordHash;
        protected readonly string vatNumber;
        protected readonly string secretKey;

        public BaseClient(string username, string password, string VATNumber, string secretKey, string baseUrl = Constants.TEST)
        {
            if (string.IsNullOrEmpty(username))  throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password))  throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(VATNumber)) throw new ArgumentNullException("VATNumber");
            if (string.IsNullOrEmpty(baseUrl))   throw new ArgumentNullException("baseUrl");
            if (string.IsNullOrEmpty(secretKey)) throw new ArgumentNullException("secretKey");
            if(!Validation.Validators.IsValidVatNumber(VATNumber)) throw new ArgumentException("VAT number is not valid", "VATNumber");
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out this.baseUrl)) throw new UriFormatException("Invalid base url");

            using (var sha = SHA512.Create())
            {
                this.passwordHash = sha.ComputeHash((BinaryString)password);
            }

            this.username = username;
            this.vatNumber = VATNumber;
            this.secretKey = secretKey;
        }

        protected virtual string GenerateRequestId() => Guid.NewGuid().ToString().Replace("-", "");

        protected BasicHeaderType BuildHeader()
        {
            return new BasicHeaderType
            {
                Timestamp = DateTime.Now,
                RequestId = GenerateRequestId(),
                HeaderVersion = Constants.HEADERVERSION,
                RequestVersion = Constants.REQUESTVERSION
            };
        }

        protected UserHeaderType BuildUser(BasicHeaderType header)
        {
            if (header == null) throw new ArgumentNullException("header");
            using (var sha = SHA512.Create())
            {
                BinaryString signatureSource = header.RequestId + header.Timestamp.ToUniversalTime().ToString("yyyyMMddHHmmss") + secretKey;
                HexString sig = sha.ComputeHash(signatureSource);
                return new UserHeaderType
                {
                    PasswordHash = passwordHash,
                    User = username,
                    VATNumber = vatNumber,
                    RequestSignature = sig
                };
            }
        }

        protected TResponse Request<TResponse, TRequest>(string destinationUrl, TRequest request)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(baseUrl, new Uri(destinationUrl, UriKind.Relative)));
            byte[] bytes = Encoding.UTF8.GetBytes(XmlConverter.ToXml(request));
            httpRequest.ContentType = "text/xml; encoding='utf-8'";
            httpRequest.ContentLength = bytes.Length;
            httpRequest.Method = "POST";
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        return XmlConverter.FromXml<TResponse>(responseStream);
                    }
                }
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(respStream, Encoding.Default, false, 512, true))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text, webex);
                    }
                }
            }
            return default;
        }

        protected T BuildRequest<T>() where T : BasicRequestType, new()
        {
            var header = BuildHeader();
            var result = new T()
            {
                Header = header,
                User = BuildUser(header)
            };
            return result;
        }
    }
}
