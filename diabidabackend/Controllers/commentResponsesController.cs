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
    public class commentResponsesController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // PUT: api/commentResponses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutcommentResponse(int id, commentResponse commentResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != commentResponse.id)
            {
                return BadRequest();
            }

            db.Entry(commentResponse).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!commentResponseExists(id))
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

        // POST: api/commentResponses
        [ResponseType(typeof(commentResponse))]
        public IHttpActionResult PostcommentResponse(commentResponse commentResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            commentResponse.recordDate = DateTime.Now;
            commentResponse.active = true;
            db.commentResponses.Add(commentResponse);

            var userFinded = db.users.FirstOrDefault(u => u.id == commentResponse.userId && u.active);
            if (userFinded != null)
            {
                //solo si la respuesta es de un usuario tipo paciente
                if (userFinded.userTypeId == UserDTO.USER_TYPE_PATIENT)
                {
                    var comment = db.comments.Where(c => c.id == commentResponse.commentId && c.active).ToList().LastOrDefault();
                    if (comment != null)
                    {
                        var specialistPatient = db.specialistByPatients.Where(ce => ce.patientId == userFinded.id && ce.active).ToList().LastOrDefault();
                        if (specialistPatient != null)
                        {
                            notification newNotification = new notification();
                            newNotification.notificationText = "El paciente " 
                                + userFinded.name
                                + " respondió un comentario del reporte entre las fechas [" + comment.startDate.ToString("yyyy/MM/dd") + "-" + comment.endDate.ToString("yyyy/MM/dd") + "]: " 
                                + commentResponse.responseText;
                            newNotification.typeId = NotificationDTO.NOTIFICATION_TYPE_COMMENT;
                            newNotification.specialistId = specialistPatient.specialistId;
                            newNotification.recordDate = DateTime.Now;
                            newNotification.commentId = comment.id;
                            newNotification.active = true;

                            db.notifications.Add(newNotification);
                            SendNotification.Send(specialistPatient.user.tokenFCM, "Diabida", newNotification.notificationText);
                        }
                    }
                }
            }

            db.SaveChanges();
            return Ok(commentResponse);
        }

        // DELETE: api/commentResponses/5
        [ResponseType(typeof(commentResponse))]
        public IHttpActionResult DeletecommentResponse(int id)
        {
            commentResponse commentResponse = db.commentResponses.Find(id);
            if (commentResponse == null)
            {
                return NotFound();
            }

            commentResponse.active = false;
            db.SaveChanges();

            return Ok(commentResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool commentResponseExists(int id)
        {
            return db.commentResponses.Count(e => e.id == id) > 0;
        }
    }
}