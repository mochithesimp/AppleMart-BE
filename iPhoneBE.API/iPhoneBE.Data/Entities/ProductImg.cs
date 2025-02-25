﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Interfaces;

namespace iPhoneBE.Data.Model
{
    public class ProductImg : IBaseEntity
    {
        [Key]
        public int ProductImgID { get; set; }

        [ForeignKey("ProductItem")]
        [Required(ErrorMessage = "Product Item ID is required.")]
        public int ProductItemID { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(1000, ErrorMessage = "Image URL cannot exceed 1000 characters.")]
        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ProductItem ProductItem { get; set; }
    }
}

