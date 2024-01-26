using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class SerialOld : BEntity {

        public string Sucursal { get; set; }
        public int? NumeroNota { get; set; }
        public DateTime? FechaNota { get; set; }
        public int? NumeroEntrega { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public string ItemCode { get; set; }
        public string Descripcion { get; set; }
        public string SerialNumber { get; set; }

        public SerialOld() { }

    }
}
