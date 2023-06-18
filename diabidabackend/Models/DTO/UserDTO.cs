using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class UserDTO
    {
        public static string SIN_DATOS = "SIN DATOS";
        public static int USER_TYPE_SPECIALIST = 1;
        public static int USER_TYPE_PATIENT = 2;

        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public System.DateTime birthday { get; set; }
        public string pictureUrl { get; set; }
        public int userTypeId { get; set; }
        public float weight { get; set; }
        public float height { get; set; }
        public string clinic { get; set; }
        public string specialist { get; set; }
        public bool active { get; set; }
        public string tokenFCM { get; set; }
        public int bpmStartRange { get; set; }
        public int bpmEndRange { get; set; }
    }
}