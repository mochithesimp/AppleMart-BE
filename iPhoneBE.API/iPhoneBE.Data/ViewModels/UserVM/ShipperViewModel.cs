using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.ViewModels.UserVM
{
    public class ShipperViewModel : UserViewModel
    {
        public int PendingOrdersCount { get; set; }
    }
}