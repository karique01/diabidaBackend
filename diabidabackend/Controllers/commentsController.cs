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
    public class commentsController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // PUT: api/comments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putcomment(int id, comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.id)
            {
                return BadRequest();
            }

            db.Entry(comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!commentExists(id))
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

        // POST: api/comments
        [ResponseType(typeof(comment))]
        public IHttpActionResult Postcomment(comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            comment.recordDate = DateTime.Now;
            comment.active = true;
            db.comments.Add(comment);

            db.SaveChanges();
            return Ok(comment);
        }

        // DELETE: api/comments/5
        [ResponseType(typeof(comment))]
        public IHttpActionResult Deletecomment(int id)
        {
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.active = false;
            db.SaveChanges();

            return Ok(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool commentExists(int id)
        {
            return db.comments.Count(e => e.id == id) > 0;
        }
    }
}