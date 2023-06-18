using diabidabackend.Extensions;
using diabidabackend.Models.DTO;
using diabidabackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace diabidabackend.Controllers
{
    [RoutePrefix("api/users")]
    public class userController : ApiController
    {
        private diabidadbEntities db = new diabidadbEntities();

        // POST: api/users/addUser
        [HttpPost]
        [Route("addUser")]
        [ResponseType(typeof(user))]
        public IHttpActionResult Postuser(user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAdded = db.users.Add(user);
            var userBpmInterval = new bpmInterval()
            {
                bpmStartRange = 60,
                bpmEndRange = 90,
                recordDate = DateTime.Now,
                userId = userAdded.id,
                active = true
            };

            var bpmIntervalAdded = db.bpmIntervals.Add(userBpmInterval);

            db.SaveChanges();
            return Ok(userAdded);
        }

        // GET: api/users/specialist/{id:int}/patients
        [HttpGet]
        [Route("specialist/{id:int}/patients")]
        public IEnumerable<PatientDTO> GetSpecialistPatients(int id)
        {
            List<PatientDTO> patients = new List<PatientDTO>();
            db.specialistByPatients
                .Include("user1")
                .Where(c => c.specialistId == id && c.active)
                .ToList()
                .ForEach(u => 
                {
                    var patientClinic = db.clinicByPatients.Where(cbp => cbp.patientId == u.user1.id && cbp.active).ToList().LastOrDefault();

                    string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;
                    int uedad = Convert.ToInt32((DateTime.Today - u.user1.birthday).Days / 365.25);

                    var weightPatient = db.weightHistories.Where(w => w.userId == u.user1.id && w.active).ToList().LastOrDefault();
                    float weightValue = weightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)weightPatient.weight;

                    var heightPatient = db.heightHistories.Where(h => h.userId == u.user1.id && h.active).ToList().LastOrDefault();
                    float heightValue = heightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)heightPatient.height;

                    var bpmPatient = db.bpmIntervals.Where(w => w.userId == u.user1.id && w.active).ToList().LastOrDefault();
                    int bpmStartValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmStartRange;
                    int bpmEndValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmEndRange;

                    patients.Add(new PatientDTO()
                    {
                        id = u.user1.id,
                        name = u.user1.name,
                        email = u.user1.email,
                        birthday = u.user1.birthday,
                        pictureUrl = u.user1.pictureUrl,
                        userTypeId = u.user1.userTypeId,
                        clinicName = uclinicName,
                        edad = uedad,
                        weight = weightValue,
                        height = heightValue,
                        tokenFCM = u.user1.tokenFCM,
                        bpmStartRange = bpmStartValue,
                        bpmEndRange = bpmEndValue,
                        specialistByPatientId = u.id
                    });
                });

            //db.specialistByPatients
            //    .Where(c => c.specialistId == id && c.active)
            //    .Select(r => r.user1)
            //    .ToList()
            //    .ForEach(u =>
            //    {
            //        var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == u.id && ce.active).ToList().LastOrDefault();

            //        string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;
            //        int uedad = Convert.ToInt32((DateTime.Today - u.birthday).Days / 365.25);

            //        var weightPatient = db.weightHistories.Where(w => w.userId == u.id && w.active).ToList().LastOrDefault();
            //        float weightValue = weightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)weightPatient.weight;

            //        var heightPatient = db.heightHistories.Where(h => h.userId == u.id && h.active).ToList().LastOrDefault();
            //        float heightValue = heightPatient == null ? PatientDTO.SIN_DATOS_NUMERIC_FLOAT : (float)heightPatient.height;

            //        var bpmPatient = db.bpmIntervals.Where(w => w.userId == u.id && w.active).ToList().LastOrDefault();
            //        int bpmStartValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmStartRange;
            //        int bpmEndValue = bpmPatient == null ? PatientDTO.SIN_DATOS_NUMERIC : bpmPatient.bpmEndRange;

            //        patients.Add(new PatientDTO()
            //        {
            //            id = u.id,
            //            name = u.name,
            //            email = u.email,
            //            birthday = u.birthday,
            //            pictureUrl = u.pictureUrl,
            //            userTypeId = u.userTypeId,
            //            clinicName = uclinicName,
            //            edad = uedad,
            //            weight = weightValue,
            //            height = heightValue,
            //            tokenFCM = u.tokenFCM,
            //            bpmStartRange = bpmStartValue,
            //            bpmEndRange = bpmEndValue,
            //            specialistByPatientId = 
            //        });
            //    });

            return patients;
        }

        // GET: api/users/specialist/{id:int}/appointments/future
        [HttpGet]
        [Route("specialist/{id:int}/appointments/future")]
        public IEnumerable<AppointmentDTO> GetSpecialistAppointmentsFuture(int id)
        {
            List<AppointmentDTO> appointments = new List<AppointmentDTO>();

            db.appointments
                .Where(a => a.specialistId == id && a.appointmentDate >= DateTime.Now)
                .OrderBy(c => c.appointmentDate)
                .ToList()
                .ForEach(a =>
                {
                    var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == a.user1.id && ce.active).ToList().LastOrDefault();
                    string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;

                    var culture = new System.Globalization.CultureInfo("es-MX");
                    var nombreDia = culture.DateTimeFormat.GetDayName(a.appointmentDate.DayOfWeek).CapitalizeFirstLetter();
                    var nombreMes = culture.DateTimeFormat.GetMonthName(a.appointmentDate.Month).CapitalizeFirstLetter();
                    string uAppointmentDateMask = nombreDia + " " + a.appointmentDate.Day + " de " + nombreMes;
                    int uedad = Convert.ToInt32((DateTime.Today - a.user1.birthday).Days / 365.25);

                    var uPatient = new PatientDTO()
                    {
                        id = a.user1.id,
                        name = a.user1.name,
                        email = a.user1.email,
                        birthday = a.user1.birthday,
                        pictureUrl = a.user1.pictureUrl,
                        userTypeId = a.user1.userTypeId,
                        clinicName = uclinicName,
                        edad = uedad
                    };

                    appointments.Add(new AppointmentDTO()
                    {
                        id = a.id,
                        patient = uPatient,
                        appointmentDate = a.appointmentDate,
                        appointmentDateMask = uAppointmentDateMask,
                        clinicName = uclinicName,
                        hour = a.appointmentDate.TimeOfDay.ToString(@"hh\:mm"),
                        minutes = a.minutes,
                        recordDate = a.recordDate
                    });
                });

            return appointments;
        }

        // GET: api/users/specialist/{id:int}/appointments/past
        [HttpGet]
        [Route("specialist/{id:int}/appointments/past")]
        public IEnumerable<AppointmentDTO> GetSpecialistAppointmentsPast(int id)
        {
            List<AppointmentDTO> appointments = new List<AppointmentDTO>();

            db.appointments
                .Where(a => a.specialistId == id && a.appointmentDate < DateTime.Now)
                .OrderByDescending(c => c.appointmentDate)
                .ToList()
                .ForEach(a =>
                {
                    var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == a.user1.id && ce.active).ToList().LastOrDefault();
                    string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;

                    var culture = new System.Globalization.CultureInfo("es-MX");
                    var nombreDia = culture.DateTimeFormat.GetDayName(a.appointmentDate.DayOfWeek).CapitalizeFirstLetter();
                    var nombreMes = culture.DateTimeFormat.GetMonthName(a.appointmentDate.Month).CapitalizeFirstLetter();
                    string uAppointmentDateMask = nombreDia + " " + a.appointmentDate.Day + " de " + nombreMes;
                    int uedad = Convert.ToInt32((DateTime.Today - a.user1.birthday).Days / 365.25);

                    var uPatient = new PatientDTO()
                    {
                        id = a.user1.id,
                        name = a.user1.name,
                        email = a.user1.email,
                        birthday = a.user1.birthday,
                        pictureUrl = a.user1.pictureUrl,
                        userTypeId = a.user1.userTypeId,
                        clinicName = uclinicName,
                        edad = uedad
                    };

                    appointments.Add(new AppointmentDTO()
                    {
                        id = a.id,
                        patient = uPatient,
                        appointmentDate = a.appointmentDate,
                        appointmentDateMask = uAppointmentDateMask,
                        clinicName = uclinicName,
                        hour = a.appointmentDate.TimeOfDay.ToString(@"hh\:mm"),
                        minutes = a.minutes,
                        recordDate = a.recordDate
                    });
                });

            return appointments;
        }

        // GET: api/users/specialist/{id:int}/notifications
        [HttpGet]
        [Route("specialist/{id:int}/notifications")]
        public IEnumerable<NotificationDTO> GetSpecialistNotifications(int id)
        {
            List<NotificationDTO> notifications = new List<NotificationDTO>();

            db.notifications
                .Where(n => n.specialistId == id)
                .OrderByDescending(n => n.recordDate)
                .ToList()
                .ForEach(n =>
                {
                    notifications.Add(new NotificationDTO()
                    {
                        id = n.id,
                        notificationText = n.notificationText,
                        typeId = n.typeId,
                        specialistId = n.specialistId,
                        recordDate = n.recordDate,
                        userHighBpmId = n.userHighBpmId == null ? 0 : n.userHighBpmId.Value,
                        commentId = n.commentId == null ? 0 : n.commentId.Value,
                        patientAssignedId = n.patientAssignedId == null ? 0 : n.patientAssignedId.Value
                    });
                });

            return notifications;
        }

        // GET: api/users/patient/{id:int}/lastheartbeat
        [HttpGet]
        [Route("patient/{id:int}/lastheartbeat")]
        public HeartBeatDTO GetLastHeartBeat(int id)
        {
            var lastHeartBeat = db.heartbeatHistories
                .Where(h => h.userId == id)
                .OrderBy(n => n.recordDate)
                .ToList()
                .LastOrDefault();

            if (lastHeartBeat == null)
                return new HeartBeatDTO()
                {
                    id = HeartBeatDTO.SIN_DATOS_NUMERIC,
                    averageBpm = HeartBeatDTO.SIN_DATOS_NUMERIC_FLOAT,
                    recordDate = new DateTime()
                };
            else
                return new HeartBeatDTO()
                {
                    id = lastHeartBeat.id,
                    averageBpm = lastHeartBeat.averageBpm,
                    recordDate = lastHeartBeat.recordDate
                };
        }

        // POST: api/users/patient/addMultipleHeartBeats
        [HttpPost]
        [Route("patient/addMultipleHeartBeats")]
        public IHttpActionResult GetLastHeartBeat(ManyHeartBeatDTO manyHeartBeatDTO)
        {
            db.heartbeatHistories.AddRange(manyHeartBeatDTO.heartbeats);
            db.SaveChanges();

            return Ok(manyHeartBeatDTO.heartbeats.LastOrDefault());
        }

        // GET: api/users/patient/{id:int}/heartbeat
        [HttpGet]
        [Route("patient/{id:int}/heartbeats")]
        public IEnumerable<HeartBeatDTO> GetHeartbeats(int id)
        {
            List<HeartBeatDTO> heartbeats = new List<HeartBeatDTO>();

            db.heartbeatHistories
                .Where(h => h.userId == id && h.active)
                .ToList()
                .ForEach(h =>
                {
                    heartbeats.Add(new HeartBeatDTO()
                    {
                        id = h.id,
                        averageBpm = h.averageBpm,
                        recordDate = h.recordDate
                    });
                });

            return heartbeats;
        }

        // GET: api/users/patient/{id:int}/heartBeatHistory/start/{dayStart:datetime}/end/{dayEnd:datetime}
        [HttpGet]
        [Route("patient/{id:int}/heartBeatHistory/start/{dayStart:datetime}/end/{dayEnd:datetime}")]
        public IEnumerable<HeartBeatDTO> GetHeartBeatHistoryFechas(int id, DateTime dayStart, DateTime dayEnd)
        {
            List<HeartBeatDTO> heartbeats = new List<HeartBeatDTO>();

            db.heartbeatHistories
                .Where(c => c.userId == id 
                && c.active 
                && DbFunctions.TruncateTime(c.recordDate).Value >= dayStart 
                && DbFunctions.TruncateTime(c.recordDate).Value <= dayEnd)
                .OrderBy(c => c.recordDate)
                .ToList()
                .ForEach(h =>
                {
                    heartbeats.Add(new HeartBeatDTO()
                    {
                        id = h.id,
                        averageBpm = h.averageBpm,
                        recordDate = h.recordDate
                    });
                });

            return heartbeats;
        }

        // GET: api/users/patient/{id:int}/comments/start/{dayStart:datetime}/end/{dayEnd:datetime}
        [HttpGet]
        [Route("patient/{id:int}/comments/start/{dayStart:datetime}/end/{dayEnd:datetime}")]
        public IEnumerable<CommentDTO> GetCommentsInFechas(int id, DateTime dayStart, DateTime dayEnd)
        {
            List<CommentDTO> comments = new List<CommentDTO>();

            db.comments
                .Where(c => c.patientId == id
                && c.active
                && DbFunctions.TruncateTime(c.startDate).Value == dayStart
                && DbFunctions.TruncateTime(c.endDate).Value == dayEnd)
                .OrderBy(c => c.recordDate)
                .ToList()
                .ForEach(c =>
                {
                    UserDTO specialist = new UserDTO()
                    {
                        id = c.user.id,
                        name = c.user.name,
                        email = c.user.email,
                        password = c.user.password,
                        birthday = c.user.birthday,
                        pictureUrl = c.user.pictureUrl,
                        userTypeId = c.user.userTypeId,
                        active = c.user.active,
                        tokenFCM = c.user.tokenFCM
                    };
                    UserDTO patient = new UserDTO()
                    {
                        id = c.user1.id,
                        name = c.user1.name,
                        email = c.user1.email,
                        password = c.user1.password,
                        birthday = c.user1.birthday,
                        pictureUrl = c.user1.pictureUrl,
                        userTypeId = c.user1.userTypeId,
                        active = c.user1.active,
                        tokenFCM = c.user1.tokenFCM
                    };

                    List<CommentResponseDTO> responses = new List<CommentResponseDTO>();
                    db.commentResponses
                    .Where(cr => cr.commentId == c.id && cr.active)
                    .ToList()
                    .ForEach(cr => {
                        UserDTO userCR = new UserDTO()
                        {
                            id = cr.id,
                            name = cr.user.name,
                            email = cr.user.email,
                            password = cr.user.password,
                            birthday = cr.user.birthday,
                            pictureUrl = cr.user.pictureUrl,
                            userTypeId = cr.user.userTypeId,
                            active = cr.user.active,
                            tokenFCM = cr.user.tokenFCM
                        };

                        responses.Add(new CommentResponseDTO()
                        {
                            id = cr.id,
                            responseText = cr.responseText,
                            commentId = cr.commentId,
                            user = userCR,
                            recordDate = cr.recordDate
                        });
                    });

                    comments.Add(new CommentDTO()
                    {
                        id = c.id,
                        commentText = c.commentText,
                        startDate = c.startDate,
                        endDate = c.endDate,
                        specialist = specialist,
                        patient = patient,
                        recordDate = c.recordDate,
                        responses = responses
                    });
                });

            return comments;
        }

        // GET: api/users/patient/{id:int}/appointments/future
        [HttpGet]
        [Route("patient/{id:int}/appointments/future")]
        public IEnumerable<AppointmentDTO> GetPatientAppointmentsFuture(int id)
        {
            List<AppointmentDTO> appointments = new List<AppointmentDTO>();

            db.appointments
                .Where(a => a.patientId == id && a.appointmentDate >= DateTime.Now)
                .OrderBy(c => c.appointmentDate)
                .ToList()
                .ForEach(a =>
                {
                    var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == a.user1.id && ce.active).ToList().LastOrDefault();
                    string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;

                    var culture = new System.Globalization.CultureInfo("es-MX");
                    var nombreDia = culture.DateTimeFormat.GetDayName(a.appointmentDate.DayOfWeek).CapitalizeFirstLetter();
                    var nombreMes = culture.DateTimeFormat.GetMonthName(a.appointmentDate.Month).CapitalizeFirstLetter();
                    string uAppointmentDateMask = nombreDia + " " + a.appointmentDate.Day + " de " + nombreMes;
                    int uedad = Convert.ToInt32((DateTime.Today - a.user1.birthday).Days / 365.25);

                    var uPatient = new PatientDTO()
                    {
                        id = a.user1.id,
                        name = a.user1.name,
                        email = a.user1.email,
                        birthday = a.user1.birthday,
                        pictureUrl = a.user1.pictureUrl,
                        userTypeId = a.user1.userTypeId,
                        clinicName = uclinicName,
                        edad = uedad
                    };

                    appointments.Add(new AppointmentDTO()
                    {
                        id = a.id,
                        patient = uPatient,
                        appointmentDate = a.appointmentDate,
                        appointmentDateMask = uAppointmentDateMask,
                        clinicName = uclinicName,
                        hour = a.appointmentDate.TimeOfDay.ToString(@"hh\:mm"),
                        minutes = a.minutes,
                        recordDate = a.recordDate
                    });
                });

            return appointments;
        }

        // GET: api/users/patient/{id:int}/appointments/past
        [HttpGet]
        [Route("patient/{id:int}/appointments/past")]
        public IEnumerable<AppointmentDTO> GetPatientAppointmentsPast(int id)
        {
            List<AppointmentDTO> appointments = new List<AppointmentDTO>();

            db.appointments
                .Where(a => a.patientId == id && a.appointmentDate < DateTime.Now)
                .OrderByDescending(c => c.appointmentDate)
                .ToList()
                .ForEach(a =>
                {
                    var patientClinic = db.clinicByPatients.Where(ce => ce.patientId == a.user1.id && ce.active).ToList().LastOrDefault();
                    string uclinicName = patientClinic == null ? PatientDTO.SIN_DATOS : patientClinic.clinic.name;

                    var culture = new System.Globalization.CultureInfo("es-MX");
                    var nombreDia = culture.DateTimeFormat.GetDayName(a.appointmentDate.DayOfWeek).CapitalizeFirstLetter();
                    var nombreMes = culture.DateTimeFormat.GetMonthName(a.appointmentDate.Month).CapitalizeFirstLetter();
                    string uAppointmentDateMask = nombreDia + " " + a.appointmentDate.Day + " de " + nombreMes;
                    int uedad = Convert.ToInt32((DateTime.Today - a.user1.birthday).Days / 365.25);

                    var uPatient = new PatientDTO()
                    {
                        id = a.user1.id,
                        name = a.user1.name,
                        email = a.user1.email,
                        birthday = a.user1.birthday,
                        pictureUrl = a.user1.pictureUrl,
                        userTypeId = a.user1.userTypeId,
                        clinicName = uclinicName,
                        edad = uedad
                    };

                    appointments.Add(new AppointmentDTO()
                    {
                        id = a.id,
                        patient = uPatient,
                        appointmentDate = a.appointmentDate,
                        appointmentDateMask = uAppointmentDateMask,
                        clinicName = uclinicName,
                        hour = a.appointmentDate.TimeOfDay.ToString(@"hh\:mm"),
                        minutes = a.minutes,
                        recordDate = a.recordDate
                    });
                });

            return appointments;
        }

        // POST: api/users/patient/addClinicByPatient
        [HttpPost]
        [Route("patient/addClinicByPatient")]
        public IHttpActionResult AddClinicByPatient(clinicByPatient clinicByPatient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            clinicByPatient.active = true;
            clinicByPatient.recordDate = DateTime.Now;
            db.clinicByPatients.Add(clinicByPatient);
            db.SaveChanges();

            return Ok(clinicByPatient);
        }

        // GET: api/users/{id:int}/patientsAvailablesBySpecialist
        [HttpGet]
        [Route("{id:int}/patientsAvailablesBySpecialist")]
        public IEnumerable<PatientDTO> GetPatientsAvailablesBySpecialist(int id)
        {
            List<PatientDTO> patients = new List<PatientDTO>();

            var currentPatients = db.specialistByPatients
                .Where(c => c.specialistId == id && c.active)
                .Select(r => r.user1);

            db.users.Where(
                u => u.userTypeId == 2
                && u.active
                && !currentPatients.Any(c => c.id == u.id)
            ).ToList()
            .ForEach(u => 
            {
                patients.Add(new PatientDTO()
                {
                    id = u.id,
                    name = u.name,
                    email = u.email,
                    birthday = u.birthday,
                    pictureUrl = u.pictureUrl,
                    userTypeId = u.userTypeId,
                    clinicName = "",
                    edad = -1,
                    weight = -1,
                    height = -1
                });
            });

            return patients;
        }

        // POST: api/users/addSpecialistByResponsable
        [HttpPost]
        [Route("addSpecialistByResponsable")]
        public IHttpActionResult AddSpecialistByResponsable(specialistByPatient specialistByResponsable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            specialistByResponsable.active = true;
            specialistByResponsable.recordDate = DateTime.Now;
            db.specialistByPatients.Add(specialistByResponsable);
            db.SaveChanges();

            return Ok(specialistByResponsable);
        }

        // POST: api/users/addRequestBySpecialist
        [HttpPost]
        [Route("addRequestBySpecialist")]
        public IHttpActionResult AddRequestBySpecialist(specialistByPatient specialistByResponsable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user userSpecialist = db.users.FirstOrDefault(u => u.id == specialistByResponsable.specialistId);
            string specialistName = userSpecialist == null ? "NO SE ENCONTRÓ NOMBRE" : userSpecialist.name;
            string specialistUrl = userSpecialist == null ? "NO SE ENCONTRÓ URL" : userSpecialist.pictureUrl;

            notification not = new notification();
            not.typeId = 3;
            not.notificationText = 
                "El especialista " + specialistName + " le envió una solicitud para ser su doctor. Revise su apartado de perfil y acepte o rechace la petición " +
                "=%=uwu#" + specialistUrl +
                "=%=uwu#" + specialistName;
            not.specialistId = specialistByResponsable.specialistId;
            not.patientAssignedId = specialistByResponsable.patientId;
            not.active = true;
            not.recordDate = DateTime.Now;
            db.notifications.Add(not);
            db.SaveChanges();

            return Ok(specialistByResponsable);
        }

        // GET: api/users/{id:int}/GetUltimaSolicitud
        [HttpGet]
        [Route("{id:int}/GetUltimaSolicitud")]
        public NotificationDTO GetUltimaSolicitud(int id)
        {
            var n = db.notifications.Where(no => no.patientAssignedId == id && no.typeId == 3 && no.active).ToList().LastOrDefault();
            NotificationDTO notification = new NotificationDTO() { id = -1 };

            if (n != null)
            {
                notification = new NotificationDTO()
                {
                    id = n.id,
                    notificationText = n.notificationText,
                    typeId = n.typeId,
                    specialistId = n.specialistId,
                    recordDate = n.recordDate,
                    userHighBpmId = n.userHighBpmId == null ? 0 : n.userHighBpmId.Value,
                    commentId = n.commentId == null ? 0 : n.commentId.Value,
                    patientAssignedId = n.patientAssignedId == null ? 0 : n.patientAssignedId.Value,
                    active = false
                };
            }

            return notification;
        }

        // POST: api/users/aceptarSolicitud
        [HttpPost]
        [Route("aceptarSolicitud")]
        public IHttpActionResult aceptarSolicitud(NotificationDTO notificationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (notificationDTO.active == true)
            {
                specialistByPatient sbp = new specialistByPatient();
                sbp.specialistId = notificationDTO.specialistId;
                sbp.patientId = notificationDTO.patientAssignedId;
                sbp.recordDate = DateTime.Now;
                sbp.active = notificationDTO.active;
                db.specialistByPatients.Add(sbp);
                db.SaveChanges();
            }
            var not = db.notifications.FirstOrDefault(n => n.id == notificationDTO.id);
            not.active = false;
            db.SaveChanges();

            return Ok(new { ok = "ok" });
        }

        // POST: api/users/tokenFCM
        [HttpPost]
        [Route("tokenFCM")]
        public IHttpActionResult PostTokenFcm(TokenFcmDTO tokenFcmDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userFinded = db.users.FirstOrDefault(u => u.id == tokenFcmDTO.userId);
            if (userFinded != null)
            {
                userFinded.tokenFCM = tokenFcmDTO.token;
                db.SaveChanges();
                return Ok(userFinded);
            }
            else
            {
                return Conflict();
            }
        }

        // GET: api/users/specialist/{id:int}/DeleteAllNotifications
        [HttpDelete]
        [Route("specialist/{id:int}/DeleteAllNotifications")]
        public IHttpActionResult DeleteAllNotifications(int id)
        {
            db.notifications
                .Where(n => n.specialistId == id && n.active)
                .ToList()
                .ForEach(n => n.active = false);

            db.SaveChanges();

            return Ok(new { result = "Ok" });
        }




        // PUT: api/users/resetPassword
        [HttpPut]
        [Route("resetPassword")]
        public IHttpActionResult PostResetPassword(NewPasswordDTO np)
        {
            user userFinded = db.users.FirstOrDefault(u => u.id == np.userId);

            if (userFinded != null)
            {
                userFinded.password = np.newPassword;
                db.SaveChanges();
                return Ok(new { response = "Ok" });
            }
            else
            {
                return Conflict();
            }
        }

        // POST: api/users/sendResetPasswordEmail
        [HttpPost]
        [Route("sendResetPasswordEmail")]
        public IHttpActionResult PostSendResetPasswordEmail(ResetPasswordDTO rp)
        {
            try
            {
                rp.email = rp.email.ToLower();
                //user userFinded = db.users.FirstOrDefault(u => u.email == rp.email && u.dni == rp.dni);
                user userFinded = db.users.FirstOrDefault(u => u.email.ToLower() == rp.email);

                if (userFinded != null)
                {
                    string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string charsLower = "abcdefghijklmnopqrstuvwxyz";
                    string numbers = "0123456789";
                    string newPassword = "";

                    //creo la nueva contraseña
                    StringBuilder sbnp = new StringBuilder();
                    Random r = new Random();

                    sbnp.Append(numbers[r.Next(0, numbers.Length)]);
                    sbnp.Append(charsUpper[r.Next(0, charsUpper.Length)]);
                    for (int i = 0; i < 7; i++)
                        sbnp.Append(charsLower[r.Next(0, charsLower.Length)]);

                    newPassword = sbnp.ToString();
                    userFinded.password = newPassword;
                    db.SaveChanges();

                    string messageText = "Su nueva contraseña es: " + newPassword;
                    EnviaMail.Enviar(rp.email, messageText);
                    return Ok(new { response = "Ok" });
                }
                else
                {
                    return Conflict();
                }
            }
            catch (Exception e)
            {
                return Ok(new { error = e.StackTrace });
            }
        }
    }
}
