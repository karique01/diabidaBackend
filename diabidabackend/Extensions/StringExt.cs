﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Extensions
{
    public static class StringExt
    {
        public static string CapitalizeFirstLetter(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
    }
}