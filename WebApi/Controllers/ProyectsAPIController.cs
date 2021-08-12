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
using WebApi.Data;
using WebApi.Helper;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ProyectsAPIController : ApiController
    {
        private TaskWebApiContext db = new TaskWebApiContext();

        [HttpGet]
        [Route("api/proyects/")]
        public IHttpActionResult GetProyects(HttpRequestMessage request)
        {
            string token = Utils.GetHeaderElement("token", request);

            var proyects = (from proyect in db.Proyects
                            where proyect.UserId == token
                            select proyect).ToList();

            return Ok(proyects);
        }

        [HttpPost]
        [Route("api/proyects/")]
        [ResponseType(typeof(Proyect))]
        public IHttpActionResult CreateProyect(Proyect proyect, HttpRequestMessage request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = Utils.GetHeaderElement("token", request);

            if(!Utils.IsUserExist(token, db))
            {
                return BadRequest("Usuario no permitido");
            }

            proyect.CreatedAt = DateTime.Now;
            proyect.UserId = token;

            db.Proyects.Add(proyect);
            db.SaveChanges();

            return Ok(proyect);
        }

        [HttpDelete]
        [Route("api/proyects/{id}")]
        public IHttpActionResult DeleteProyect(int id, HttpRequestMessage request)
        {
            var token = Utils.GetHeaderElement("token", request);

            if (!Utils.IsUserExist(token, db))
            {
                return BadRequest("Usuario no permitido");
            }

            var proyect = (from proy in db.Proyects
                           where proy.Id == id
                           select proy).FirstOrDefault();
            var tasks = (from task in db.Tasks
                         where task.ProyectId == id.ToString()
                         select task).ToList();

            if(proyect.UserId != token)
            {
                return BadRequest("Usuario no permitido");
            }

            db.Proyects.Remove(proyect);

            db.Tasks.RemoveRange(tasks);

            db.SaveChanges();
            return Ok(proyect);
        }
    }
}