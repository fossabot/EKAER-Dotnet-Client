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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EKAER.Validation
{
    public static class Validators
    {
        public static void Validate(this QueryParamsType queryParams)
        {
            int maxRowNum = 0;
            if (queryParams == null) throw new ArgumentNullException("queryParams");
            if (queryParams.InsertFromDate > DateTime.Now) throw new ArgumentOutOfRangeException("InsertFromDate");
            if (queryParams.InsertFromDate > queryParams.InsertToDate) throw new ArgumentException("InsertFromDate > InsertToDate");
            if (queryParams.InsertToDate.Subtract(queryParams.InsertFromDate).TotalDays > 30) throw new ArgumentException("Interval must less or equal to 30");
            if (!string.IsNullOrEmpty(queryParams.MaxRowNum) && !int.TryParse(queryParams.MaxRowNum, out maxRowNum)) throw new ArgumentException("MaxRowNum is not a number");
            if (maxRowNum > 1000 || maxRowNum < 1) throw new ArgumentException("MaxRowNum must be between 1 and 1000");
            if (!string.IsNullOrEmpty(queryParams.OrderNumber) && queryParams.OrderNumber.Length > 50) throw new ArgumentException("OrderNumber must be shorter than 51 characters");
            if (!string.IsNullOrEmpty(queryParams.PlateNumber) && !Regex.IsMatch(queryParams.PlateNumber, "[A-Z0-9ÖŐÜŰ]{4,15}")) throw new ArgumentException("Invalid Plate Number");
        }        

        public static bool IsValidVatNumber(string vatnumber)
        {
            return !string.IsNullOrEmpty(vatnumber) && Regex.IsMatch(vatnumber, @"[0-9A-Z\-]{1,15}");
        }

        public static bool IsValidCountryCode(string country)
        {
            return !string.IsNullOrEmpty(country) && Regex.IsMatch(country, "[A-Z]{1,3}");
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            return !string.IsNullOrEmpty(phone) && Regex.IsMatch(phone, @"(((\+)|(00))[0-9]{8,14})|(06[0-9]{1,2}[0-9]{6,7})");
        }

        public static bool IsValidEmailAddress(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, @"[A-Za-z0-9._%\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,4}");
        }

        public static bool IsValidZipCode(string zipCode)
        {
            return !string.IsNullOrEmpty(zipCode) && Regex.IsMatch(zipCode, @"([A-Z0-9 \-]{2,7})|()");
        }
        public static bool IsValidTradeCardNumber(string tcn)
        {
            return string.IsNullOrEmpty(tcn) || Regex.IsMatch(tcn, "[A-Z0-9]{2,20}");
        }
        public static bool IsValidVTSZ(string productVtsz)
        {
            return string.IsNullOrEmpty(productVtsz) || Regex.IsMatch(productVtsz, "[0-9]{4,8}");
        }
    }
}
