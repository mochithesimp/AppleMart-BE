using System;
using System.Collections.Generic;

namespace iPhoneBE.Data.ViewModels.AttributeDTO
{
    public class AttributeViewModel
    {
        public int AttributeID { get; set; }
        public string? AttributeName { get; set; }
        public string? DataType { get; set; }
        public int CategoryID { get; set; }
    }
}