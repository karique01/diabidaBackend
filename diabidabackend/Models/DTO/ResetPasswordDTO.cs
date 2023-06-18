using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class ResetPasswordDTO
    {
        public string email { get; set; }
        public string nombreCompleto { get; set; }
    }
} 