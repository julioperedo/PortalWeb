using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Models
{
    public class UserFilters
    {
        public string CardCode { get; set; }
        public long? ProfileCode { get; set; }
        public string Name { get; set; }
    }

}