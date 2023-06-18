using diabidabackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class AppointmentDTO
    {
        public int id { get; set; }
        public PatientDTO patient { get; set; }
        public DateTime appointmentDate { get; set; }
        public string appointmentDateMask { get; set; }
        public string clinicName { get; set; }
        public string hour { get; set; }
        public int minutes { get; set; }
        public DateTime recordDate { get; set; }
    }
}