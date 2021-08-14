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
using BC = BCrypt.Net.BCrypt;

namespace WebApi.Controllers
{
    public class UsersAPIController : ApiController
    {
        private TaskWebApiContext db = new TaskWebApiContext();

        #region GetUser
        // GET: api/UsersAPI
        public IEnumerable<Object> GetUsers()
        {
            return (from user in db.Users
                    select new {
                        Email = user.Email,
                        Name = user.Name,
                        Lastname = user.Lastname,
                        Id = user.Id
                    }).ToList();
        }

        // GET: api/UsersAPI/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { 
                user.Email,
                user.Name,
                user.Lastname,
                user.Id
            });
        }
        #endregion

        // POST: api/UsersAPI
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userWithTheEmail = (from us in db.Users
                                    where us.Email == user.Email
                                    select us).ToList();
            if(userWithTheEmail.Count() > 0)
            {
                return BadRequest("Existe un usuario con ese correo.");
            }

            user.Password = BC.HashPassword(user.Password);

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }
    }
}