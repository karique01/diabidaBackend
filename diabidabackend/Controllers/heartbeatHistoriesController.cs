using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using diabidabackend.Models.DTO;
using diabidabackend.Models.EF;

namespace diabidabackend.Controllers
{
    public class heartbeatHistoriesController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // PUT: api/heartbeatHistories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutheartbeatHistory(int id, heartbeatHistory heartbeatHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != heartbeatHistory.id)
            {
                return BadRequest();
            }

            db.Entry(heartbeatHistory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!heartbeatHistoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/heartbeatHistories
        [ResponseType(typeof(heartbeatHistory))]
        public IHttpActionResult PostheartbeatHistory(heartbeatHistory heartbeatHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            heartbeatHistory.active = true;
            db.heartbeatHistories.Add(heartbeatHistory);

            var userFinded = db.users.FirstOrDefault(u => u.id == heartbeatHistory.userId && u.active);
            if (userFinded != null)
            {
                var bpmPatient = db.bpmIntervals.Where(b => b.userId == userFinded.id && b.active).ToList().LastOrDefault();
                if (bpmPatient != null)
                {
                    int bpmStartValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmStartRange;
                    int bpmEndValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmEndRange;

                    if ((int)heartbeatHistory.averageBpm < bpmStartValue || (int)heartbeatHistory.averageBpm > bpmEndValue)
                    {
                        var specialistPatient = db.specialistByPatients.Where(ce => ce.patientId == userFinded.id && ce.active).ToList().LastOrDefault();
                        if (specialistPatient != null)
                        {
                            notification newNotification = new notification();
                            newNotification.notificationText = "El paciente "
                                + userFinded.name + " ha reportado un BPM de valor "
                                + ((int)heartbeatHistory.averageBpm) + " el cual está fuera de su rango ["
                                + bpmStartValue + "-" + bpmEndValue + "] en la fecha " + heartbeatHistory.recordDate.ToString("yyyy/MM/dd HH:mm:ss");
                            newNotification.typeId = NotificationDTO.NOTIFICATION_TYPE_BPM;
                            newNotification.specialistId = specialistPatient.specialistId;
                            newNotification.recordDate = heartbeatHistory.recordDate;
                            newNotification.userHighBpmId = userFinded.id;
                            newNotification.active = true;

                            db.notifications.Add(newNotification);
                            SendNotification.Send(specialistPatient.user.tokenFCM, "Diabida", newNotification.notificationText);
                        }
                    }
                }
            }

            db.SaveChanges();

            return Ok(heartbeatHistory);
        }

        // DELETE: api/heartbeatHistories/5
        [ResponseType(typeof(heartbeatHistory))]
        public IHttpActionResult DeleteheartbeatHistory(int id)
        {
            heartbeatHistory heartbeatHistory = db.heartbeatHistories.Find(id);
            if (heartbeatHistory == null)
            {
                return NotFound();
            }

            heartbeatHistory.active = false;
            db.SaveChanges();

            return Ok(heartbeatHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool heartbeatHistoryExists(int id)
        {
            return db.heartbeatHistories.Count(e => e.id == id) > 0;
        }
    }
}