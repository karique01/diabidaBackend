using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class NotificationDTO
    {
        public static int NOTIFICATION_TYPE_BPM = 1;
        public static int NOTIFICATION_TYPE_COMMENT = 2;

        public int id { get; set; }
        public string notificationText { get; set; }
        public int typeId { get; set; }
        public int specialistId { get; set; }
        public System.DateTime recordDate { get; set; }
        public int userHighBpmId { get; set; }
        public int commentId { get; set; }
        public int patientAssignedId { get; set; }
        public bool active { get; set; }
    }
}