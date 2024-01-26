using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class Sales : BEntity {
        public string Sucursal { get; set; }
        public string Almacen { get; set; }
        public int NroNotaVenta { get; set; }
        public int? NroOrdenVenta { get; set; }
        public string NotaCliente { get; set; }
        public DateTime Fecha { get; set; }
        public string CodCliente { get; set; }
        public string NombreCliente { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public decimal Margen { get; set; }
        public decimal MargenPorcentage { get; set; }
        public decimal TotalCalculo { get; set; }
        public decimal Porcentage { get; set; }
        public decimal StockValue { get; set; }
        public decimal AuthorizedOV { get; set; }
        public decimal TotalToday { get; set; }
        public decimal TotalOpen { get; set; }
        public string ItemCode { get; set; }
        public string Descripcion { get; set; }
        public string Linea { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public string CodVendedor { get; set; }
        public string Vendedor { get; set; }
        public int Quantity { get; set; }
        public string NotaEntrega { get; set; }
        public string NoOrdenVenta { get; set; }
        public string NoFactura { get; set; }
        public string NoNotas { get; set; }
        public string NoAutorizacion { get; set; }
        public int? NoNotaEntrega { get; set; }
        public string NoteDates { get; set; }

        public Sales() { }
    }
}
