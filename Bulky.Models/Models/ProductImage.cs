﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class ProductImage
    {
     
        public int id { get; set; }
        [Required]
        public string ImageURL { get; set; }

        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
      
        public Product Product { get; set; }
    }
}
