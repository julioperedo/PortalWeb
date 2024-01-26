using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class User : BEntity
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
}
