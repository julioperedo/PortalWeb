using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class SalesResume : BEntity
    {

        public string Sucursal { get; set; }
        public string Almacen { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public DateTime Fecha { get; set; }
        public string MesLiteral { get; set; }
        public string Cliente { get; set; }
        public string ItemCode { get; set; }
        public string Descripcion { get; set; }
        public int Factura { get; set; }
        //public string ItemName { get; set; }
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public string Linea { get; set; }
        public string Vendedor { get; set; }
        public string GP { get; set; }
        public decimal Total { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Margen { get; set; }
        public decimal MargenPorcentaje { get; set; }
        public decimal TotalCalculo { get; set; }

    }
}
