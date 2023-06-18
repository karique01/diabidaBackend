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
    public class bpmIntervalsController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // POST: api/bpmIntervals
        [ResponseType(typeof(bpmInterval))]
        public IHttpActionResult PostbpmInterval(bpmInterval bpmInterval)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bpmInterval.recordDate = DateTime.Now;
            bpmInterval.active = true;
            db.bpmIntervals.Add(bpmInterval);
            db.SaveChanges();

            return Ok(bpmInterval);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool bpmIntervalExists(int id)
        {
            return db.bpmIntervals.Count(e => e.id == id) > 0;
        }
    }
}