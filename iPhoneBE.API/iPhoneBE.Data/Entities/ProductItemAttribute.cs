﻿using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Entities
{
    public class ProductItemAttribute
    {

        [Key]
        public int ProductItemAttributeID { get; set; }

        public int ProductItemID { get; set; }

        public int AttributeID { get; set; }

        public string Value { get; set; }

        [ForeignKey("ProductItemID")]
        public virtual ProductItem ProductItem { get; set; }

        [ForeignKey("AttributeID")]
        public virtual Attribute Attribute { get; set; }
    }
}
