﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class TokenFcmDTO
    {
        public int userId { get; set; }
        public string token { get; set; }
    }
}