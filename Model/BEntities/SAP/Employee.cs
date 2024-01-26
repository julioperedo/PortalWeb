using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => FirstName + (LastName != null && LastName.Trim().Length > 0 ? " " + LastName : "");
        public DateTime StartDate { get; set; }
        public DateTime? TermDate { get; set; }
        public string Picture { get; set; }
        public string Path { get; set; }
        public string Position { get; set; }
        public string ShortName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Security.User User { get; set; }

        public bool IsAvailable
        {
            get
            {
                return !TermDate.HasValue;
            }
        }
        public Employee() { }
    }

    public enum relEmployee
    {
        User
    }
}
