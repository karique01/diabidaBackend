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
    public class clinicsController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // GET: api/clinics
        public IQueryable<clinic> Getclinics()
        {
            return db.clinics;
        }

        //// PUT: api/clinics/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult Putclinic(int id, clinic clinic)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != clinic.id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(clinic).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!clinicExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/clinics
        //[ResponseType(typeof(clinic))]
        //public IHttpActionResult Postclinic(clinic clinic)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.clinics.Add(clinic);
        //    db.SaveChanges();

        //    return Ok(clinic);
        //}
        //
        //// DELETE: api/clinics/5
        //[ResponseType(typeof(clinic))]
        //public IHttpActionResult Deleteclinic(int id)
        //{
        //    clinic clinic = db.clinics.Find(id);
        //    if (clinic == null)
        //    {
        //        return NotFound();
        //    }

        //    clinic.active = false;
        //    db.SaveChanges();

        //    return Ok(clinic);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool clinicExists(int id)
        {
            return db.clinics.Count(e => e.id == id) > 0;
        }
    }
}