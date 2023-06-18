using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class NewPasswordDTO
    {
        public int userId { get; set; }
        public string newPassword { get; set; }
    }
}