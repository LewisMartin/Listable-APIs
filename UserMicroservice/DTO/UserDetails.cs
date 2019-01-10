﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.UserMicroservice.DTO
{
    public class UserDetails
    {
        public int Id { get; set; }

        public string SubjectId { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
