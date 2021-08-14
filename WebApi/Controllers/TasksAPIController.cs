using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Data;
using WebApi.Helper;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksAPIController : ApiController
    {
        TaskWebApiContext db = new TaskWebApiContext();

        [HttpGet]
        [Route("api/tasks/{id}")]
        public IHttpActionResult GetTasks(string id, HttpRequestMessage request)
        {
            string token = Utils.GetHeaderElement("token", request);

            if (!Utils.IsUserExist(token, db))
            {
                return BadRequest("Usuario no permitido");
            }

            var tasks = (from task in db.Tasks
                         where (task.ProyectId.ToString() == id && task.UserId.ToString() == token)
                         select task).ToList();

            return Ok(tasks);
        }

        [HttpPost]
        [Route("api/tasks/{id}")]
        public IHttpActionResult PostTaks(Task task, string id, HttpRequestMessage request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token = Utils.GetHeaderElement("token", request);

            if(!Utils.IsUserExist(token, db))
            {
                return BadRequest("Usuario no permitido");
            }

            task.ProyectId = id;
            task.UserId = token;

            db.Tasks.Add(task);
            db.SaveChanges();


            return Ok(task);
        }

        [HttpPut]
        [Route("api/tasks/{id}")]
        public IHttpActionResult PutTaks(Task task, int id, HttpRequestMessage request)
        {
            string token = Utils.GetHeaderElement("token", request);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if(task.UserId != token)
            //{
            //    return BadRequest("El usuario no tiene permiso.");
            //}

            var taskS = (from tas in db.Tasks
                        where tas.Id == id
                        select tas).ToList();

            if(taskS.Count == 0)
            {
                return NotFound("Tarea no existe");
            }

            task.Title = taskS.FirstOrDefault().Title;
            task.Id = id;
            task.ProyectId = taskS.FirstOrDefault().ProyectId;
            task.UserId = taskS.FirstOrDefault().UserId;

            db.Entry(taskS.FirstOrDefault()).CurrentValues.SetValues(task);
            db.SaveChanges();

            return Ok(task);

        }

        private IHttpActionResult NotFound(string v)
        {
            throw new NotImplementedException();
        }
    }
}