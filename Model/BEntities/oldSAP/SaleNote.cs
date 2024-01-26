using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class SaleNote : BEntity
    {

        public string BD { get; set; }
        public string Direccion { get; set; }
        public int DocNum { get; set; }
        public string BaseRef { get; set; }
        public string Almacen { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string unitMsr { get; set; }
        public decimal PriceAfVAT { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public string U_TPOGTA { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string NumAtCard { get; set; }
        public decimal DiscSum { get; set; }
        public decimal VatSum { get; set; }
        public decimal DocTotal { get; set; }
        public decimal PaidToDate { get; set; }
        public string Via { get; set; }
        public string Terminos { get; set; }
        public string Vendedor { get; set; }
        public string Comments { get; set; }
        public string JrnlMemo { get; set; }
        public int TransId { get; set; }
        public short Series { get; set; }
        public string U_FACTURAR { get; set; }
        public string U_NIT2 { get; set; }
        public string Telefono { get; set; }
        public string Correlativo_Ant { get; set; }
        public string Correlativo { get; set; }
        public string Incoterms { get; set; }
        public string Nombre_PC { get; set; }
        public string Telefono_PC { get; set; }
        public string Acreditado { get; set; }
        public string CodigoFabricante { get; set; }
        public string Celular { get; set; }
        public string Celular_PC { get; set; }
        public string Seriales { get; set; }

        public SaleNote() { }

    }
}
