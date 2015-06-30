using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastORM;

namespace FastORM.Test
{
    [Table("test_user")]
    public class User
    {
        [Column("id")]
        public int id { get; set; }
        [Column("username")]
        public string username { get; set; }
        [Column("password")]
        public string password { get; set; }
        [Column("name")]
        public string name { get; set; }
    }
}
