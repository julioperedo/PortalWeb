using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class SAPContacts : BEntity
    {
        public int Id { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
    }
}
