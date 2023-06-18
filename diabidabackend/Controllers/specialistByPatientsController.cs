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
    public class specialistByPatientsController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // DELETE: api/specialistByPatients/5
        [ResponseType(typeof(specialistByPatient))]
        public IHttpActionResult DeletespecialistByPatient(int id)
        {
            specialistByPatient specialistByPatient = db.specialistByPatients.FirstOrDefault(s => s.id == id && s.active);
            if (specialistByPatient == null)
            {
                return NotFound();
            }

            specialistByPatient.active = false;
            db.SaveChanges();

            return Ok(specialistByPatient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool specialistByPatientExists(int id)
        {
            return db.specialistByPatients.Count(e => e.id == id) > 0;
        }
    }
}