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
using diabidabackend.Models.EF;

namespace diabidabackend.Controllers
{
    public class notificationsController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // DELETE: api/notifications/5
        [ResponseType(typeof(notification))]
        public IHttpActionResult Deletenotification(int id)
        {
            notification notification = db.notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.active = false;
            db.SaveChanges();

            return Ok(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool notificationExists(int id)
        {
            return db.notifications.Count(e => e.id == id) > 0;
        }
    }
}