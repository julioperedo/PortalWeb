using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class ContactPerson : BEntity {
        public int Id { get; set; }
        public string CardCode { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Cellular { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string IdentityCard { get; set; }
        public string Enabled { get; set; }

        public ContactPerson() { }
    }
}
