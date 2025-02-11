﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iPhoneBE.Data.Entities;

namespace iPhoneBE.Data.Model
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [ForeignKey("ProductItem")]
        [Required(ErrorMessage = "Product Item ID is required.")]
        public int ProductItemID { get; set; }

        [ForeignKey("Order")]
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0.")]
        public double Price { get; set; }

        public ProductItem ProductItem { get; set; }
        public Order Order { get; set; }
        public virtual ICollection<Review> Reviews { get; set;}
    }
}
