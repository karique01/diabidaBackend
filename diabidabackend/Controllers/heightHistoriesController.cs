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
    public class heightHistoriesController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // PUT: api/heightHistories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutheightHistory(int id, heightHistory heightHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != heightHistory.id)
            {
                return BadRequest();
            }

            db.Entry(heightHistory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!heightHistoryExists(id))
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

        // POST: api/heightHistories
        [ResponseType(typeof(heightHistory))]
        public IHttpActionResult PostheightHistory(heightHistory heightHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            heightHistory.active = true;
            heightHistory.recordDate = DateTime.Now;
            db.heightHistories.Add(heightHistory);
            db.SaveChanges();

            return Ok(heightHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool heightHistoryExists(int id)
        {
            return db.heightHistories.Count(e => e.id == id) > 0;
        }
    }
}