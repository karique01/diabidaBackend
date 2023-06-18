using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class PatientDTO
    {
        public static string SIN_DATOS = "SIN DATOS";
        public static float SIN_DATOS_NUMERIC_FLOAT = -1;
        public static int SIN_DATOS_NUMERIC = 1;

        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public System.DateTime birthday { get; set; }
        public string pictureUrl { get; set; }
        public int userTypeId { get; set; }
        public string clinicName { get; set; }
        public int edad { get; set; }
        public float weight { get; set; }
        public float height { get; set; }
        public string tokenFCM { get; set; }
        public int bpmStartRange { get; set; }
        public int bpmEndRange { get; set; }
        public int specialistByPatientId { get; set; }
    }
}