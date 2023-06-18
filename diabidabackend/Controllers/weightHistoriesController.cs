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
    public class weightHistoriesController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // PUT: api/weightHistories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutweightHistory(int id, weightHistory weightHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != weightHistory.id)
            {
                return BadRequest();
            }

            db.Entry(weightHistory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!weightHistoryExists(id))
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

        // POST: api/weightHistories
        [ResponseType(typeof(weightHistory))]
        public IHttpActionResult PostweightHistory(weightHistory weightHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            weightHistory.active = true;
            weightHistory.recordDate = DateTime.Now;
            db.weightHistories.Add(weightHistory);
            db.SaveChanges();

            return Ok(weightHistory);
        }

        // DELETE: api/weightHistories/5
        [ResponseType(typeof(weightHistory))]
        public IHttpActionResult DeleteweightHistory(int id)
        {
            weightHistory weightHistory = db.weightHistories.Find(id);
            if (weightHistory == null)
            {
                return NotFound();
            }

            weightHistory.active = false;
            db.SaveChanges();

            return Ok(weightHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool weightHistoryExists(int id)
        {
            return db.weightHistories.Count(e => e.id == id) > 0;
        }
    }
}