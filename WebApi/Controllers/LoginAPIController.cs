using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Data;
using WebApi.Helper;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class LoginAPIController : ApiController
    {
        private TaskWebApiContext db = new TaskWebApiContext();

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post(Login loginUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = (from us in db.Users
                        where us.Email == loginUser.Email
                        select us).FirstOrDefault();

            if(user?.Password != Encrypt.GetPasswordEncrypt(loginUser.Password))
            {
                return BadRequest("Contraseña incorrecta o Usuario no existe");
            }

            return Ok(new {
                Id = user.Id
            });
        }
    }
}