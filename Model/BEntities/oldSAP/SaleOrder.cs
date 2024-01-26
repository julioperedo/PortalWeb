using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class SaleOrder : BEntity
    {

        public string BD { get; set; }
        public string Sucursal { get; set; }
        public string Direccion { get; set; }
        public int NumOrden { get; set; }
        public DateTime FechaOrden { get; set; }
        public string CodCliente { get; set; }
        public string NomCliente { get; set; }
        public string Comentarios { get; set; }
        public string DirFactura { get; set; }
        public string DirDestino { get; set; }
        public string CodDestino { get; set; }
        public string Incoterms { get; set; }
        public string OrdenCliente { get; set; }
        public string Correlativo { get; set; }
        public string Acreditado { get; set; }
        public string Almacen { get; set; }
        public string Vendedor { get; set; }
        public string VendedorFull { get; set; }
        public string Estado { get; set; }
        public string FactNum { get; set; }
        public string NoteDates { get; set; }
        public string Via { get; set; }
        public string CondicionesPago { get; set; }
        public string Warehouse { get; set; }
        public string Articulo { get; set; }
        public string DescArticulo { get; set; }
        public string CodigoFabricante { get; set; }
        public string Autorizado { get; set; }
        public decimal CantOrdenada { get; set; }
        public decimal CantDespachada { get; set; }
        public decimal CantAbierta { get; set; }
        public decimal Unitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal DiscSum { get; set; }
        public decimal Margen { get; set; }
        public decimal TotalCalculo { get; set; }
        public decimal TotalFacturado { get; set; }
        public int NonComplete { get; set; }

        public SaleOrder() { }

    }
}
