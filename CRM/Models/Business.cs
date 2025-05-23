﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Business
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا اسم را وارد کنید")]
        public string Name { get; set; }
    }
}
