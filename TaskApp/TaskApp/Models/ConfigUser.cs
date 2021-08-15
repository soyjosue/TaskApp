using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TaskApp.Models
{
    public class ConfigUser
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
