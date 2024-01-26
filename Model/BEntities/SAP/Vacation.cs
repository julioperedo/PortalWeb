using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Vacation
    {
        public string Id { get; set; }
        public int EmployeeId { get; set; }
        public int Author { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime Since { get; set; }
        public DateTime Until { get; set; }
        public decimal Days { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public string Comments { get; set; }
        public Vacation() { }
    }
}
