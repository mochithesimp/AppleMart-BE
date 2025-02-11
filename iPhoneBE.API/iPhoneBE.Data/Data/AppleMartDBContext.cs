using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Data
{
    public class AppleMartDBContext : DbContext
    {
        public AppleMartDBContext()
        {

        }

        public AppleMartDBContext(DbContextOptions<AppleMartDBContext> dbContextOptions) : base(dbContextOptions) 
        { 

        }



    }
}
