using diabidabackend.Models.DTO;
using diabidabackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace diabidabackend.Controllers
{
    public class loginController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // POST: api/Login
        [ResponseType(typeof(UserDTO))]
        public IHttpActionResult Post([FromBody] LoginDTO login)
        {
            user user = db.users.FirstOrDefault(u =>
                u.email == login.email &&
                u.password == login.password
            );

            if (user == null)
                return NotFound();
            if (user.active == false)
                return NotFound();

            UserDTO userFound = new UserDTO();

            //si es especialista
            if (user.userTypeId == UserDTO.USER_TYPE_SPECIALIST)
            {
                userFound = new UserDTO()
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    password = user.password,
                    birthday = user.birthday,
                    pictureUrl = user.pictureUrl,
                    userTypeId = user.userTypeId,
                    weight = PatientDTO.SIN_DATOS_NUMERIC_FLOAT,
                    height = PatientDTO.SIN_DATOS_NUMERIC_FLOAT,
                    clinic = PatientDTO.SIN_DATOS,
                    specialist = PatientDTO.SIN_DATOS,
                    active = user.active,
                    tokenFCM = user.tokenFCM,
                    bpmStartRange = PatientDTO.SIN_DATOS_NUMERIC,
                    bpmEndRange = PatientDTO.SIN_DATOS_NUMERIC
                };
            }
            //si es paciente
            else if (user.userTypeId == UserDTO.USER_TYPE_PATIENT)
            {
                var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == user.id && ce.active).ToList().LastOrDefault();
                string clinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;

                var specialistPatient = db.specialistByPatients.Where(ce => ce.patientId == user.id && ce.active).ToList().LastOrDefault();
                string specialistName = specialistPatient == null ? PatientDTO.SIN_DATOS : specialistPatient.user.name;

                var weightPatient = db.weightHistories.Where(w => w.userId == user.id && w.active).ToList().LastOrDefault();
                float weightValue = weightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)weightPatient.weight;

                var heightPatient = db.heightHistories.Where(h => h.userId == user.id && h.active).ToList().LastOrDefault();
                float heightValue = heightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)heightPatient.height;

                var bpmPatient = db.bpmIntervals.Where(w => w.userId == user.id && w.active).ToList().LastOrDefault();
                int bpmStartValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmStartRange;
                int bpmEndValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmEndRange;

                userFound = new UserDTO()
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    password = user.password,
                    birthday = user.birthday,
                    pictureUrl = user.pictureUrl,
                    userTypeId = user.userTypeId,
                    weight = weightValue,
                    height = heightValue,
                    clinic = clinicName,
                    specialist = specialistName,
                    active = user.active,
                    tokenFCM = user.tokenFCM,
                    bpmStartRange = bpmStartValue,
                    bpmEndRange = bpmEndValue
                };
            }

            return Ok(userFound);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
