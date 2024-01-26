using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class StateAccount : BEntity
    {

        public string Sucursal { get; set; }
        public string Tipo { get; set; }
        public int NoDoc { get; set; }
        public string CodProveedor { get; set; }
        public string NomProveedor { get; set; }
        public string RazonSocial { get; set; }
        public string Ciudad { get; set; }
        public int FacturaProveedor { get; set; }
        public string DocBase { get; set; }
        public DateTime Fecha { get; set; }
        public string Terminos { get; set; }
        public DateTime Vence { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public int Dias { get; set; }
        public string Estado { get; set; }
        public string FacturaFabricante { get; set; }
        public string GerenteProducto { get; set; }
        public string GerenteProductoShort { get; set; }

        public StateAccount() { }

    }
}
