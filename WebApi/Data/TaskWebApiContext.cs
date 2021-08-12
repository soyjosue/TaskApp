using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApi.Migrations;

namespace WebApi.Data
{
    public class TaskWebApiContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TaskWebApiContext() : base("name=TaskWebApiContext")
        {
            Database.SetInitializer<TaskWebApiContext>(
                new MigrateDatabaseToLatestVersion<TaskWebApiContext, Configuration>()
                );
        }

        public System.Data.Entity.DbSet<WebApi.Models.User> Users { get; set; }
        public System.Data.Entity.DbSet<WebApi.Models.Proyect> Proyects { get; set; }
        public System.Data.Entity.DbSet<WebApi.Models.Task> Tasks { get; set; }
    }
}
