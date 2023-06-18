using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class HeartBeatDTO
    {
        public static string SIN_DATOS = "SIN DATOS";
        public static float SIN_DATOS_NUMERIC_FLOAT = -1;
        public static int SIN_DATOS_NUMERIC = -1;

        public int id { get; set; }
        public double averageBpm { get; set; }
        public System.DateTime recordDate { get; set; }
    }
}