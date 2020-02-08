﻿using Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
   public class User : IEntity
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int OCID { get; set; }
        public string Email { get; set; }
        public int RoleID { get; set; }

        public virtual Role Role { get; set; }

    }
}
