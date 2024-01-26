using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Misc.Models {
    public class Staff {
        public List<Department> Departments { get; set; }

        public Staff() {
            Departments = new List<Department>();
        }
    }

    public class Department {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public List<Contact> Managers { get; set; }
        public List<Contact> Members { get; set; }

        public Department() {
            Managers = new List<Contact>();
            Members = new List<Contact>();
        }

        public Department(string DepartmentName) {
            Name = DepartmentName;
            Managers = new List<Contact>();
            Members = new List<Contact>();
        }
    }

    public class Contact {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Mail { get; set; }
        public string Photo { get; set; }
        public int? Phone { get; set; }
    }
}
