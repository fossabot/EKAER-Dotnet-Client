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

using System;
using System.Linq;

namespace EKAER.Utils
{
    public struct HexString
    {
        private readonly byte[] _value;
        public HexString(byte[] b) => _value = b;
        public int Length { get => _value.Length; }
        public HexString Reverse() => _value.Reverse().ToArray();

        public static implicit operator HexString(string s) => new HexString(StringToByteArray(s));
        public static implicit operator string(HexString s) => s._value == null ? null : BitConverter.ToString(s).Replace("-", "");
        public static implicit operator byte[] (HexString s) => s._value;
        public static implicit operator HexString(byte[] s) => new HexString(s);
        public override string ToString() => this;
        public bool SequenceEqual(HexString x)=> _value.SequenceEqual(x._value);
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}