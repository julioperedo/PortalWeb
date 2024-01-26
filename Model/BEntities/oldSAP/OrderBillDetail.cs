using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    [Serializable()]
    public class OrderBillDetail : BEntity
    {
        #region Properties

        public string Sucursal { get; set; }
        public int NumeroFactura { get; set; }
        public decimal Cantidad { get; set; }
        public string ItemCode { get; set; }
        public string CodFabricante { get; set; }
        public string Descripcion { get; set; }
        public string Impuesto { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }

        #endregion

        #region Contructors

        #endregion
    }
}
