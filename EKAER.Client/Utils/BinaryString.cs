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
using System.Linq;
using System.Text;

namespace EKAER.Utils
{
    public struct BinaryString
    {
        private readonly byte[] _value;

        public BinaryString(byte[] b)
        {
            _value = b;
        }

        public int Length => _value == null ? 0 : _value.Length;

        public static implicit operator BinaryString(string s)
        {
            return new BinaryString(s == null ? new byte[0] : Encoding.Default.GetBytes(s));
        }

        public static implicit operator string(BinaryString s)
        {
            return s._value == null ? null : Encoding.UTF8.GetString(s._value);
        }

        public static implicit operator byte[](BinaryString s)
        {
            return s._value;
        }

        public static implicit operator BinaryString(byte[] s)
        {
            return new BinaryString(s);
        }

        public bool IsReadable => _value == null ? true : _value.Count(p => char.IsControl((char)p)) == 0;

        public override string ToString()
        {
            return this;
        }
    }
}