using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    [Serializable()]
    public class OrderBill : BEntity
    {
        #region Properties

        public string BD { get; set; }
        public string Sucursal { get; set; }
        public string Direccion { get; set; }
        public int NumeroFactura { get; set; }
        public DateTime FechaFactura { get; set; }
        public string NombreProveedor { get; set; }
        public string DireccionProveedor { get; set; }
        public string DireccionDespacho { get; set; }
        public string OrdenNumero { get; set; }
        public string Referencia { get; set; }
        public string Almacen { get; set; }
        public string CodigoProveedor { get; set; }
        public string EncCompra { get; set; }
        public string TerminosPago { get; set; }
        public decimal GastosAdicionales { get; set; }
        public decimal Total { get; set; }
        public decimal Abonado { get; set; }
        public string Comentarios { get; set; }
        public string ComentariosDiario { get; set; }
        public decimal TotalLineas { get; set; }

        #endregion

        #region Contructors

        #endregion
    }
}
