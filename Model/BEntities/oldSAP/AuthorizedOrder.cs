using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class AuthorizedOrder : BEntity
    {
        public string Sucursal { get; set; }
        public string Almacen { get; set; }
        public string CardCode { get; set; }
        public string Cliente { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public string NumAtCard { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAbierto { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Estado { get; set; }
        public string Vendedor { get; set; }
        public bool Complete { get; set; }
        public int NonCompleteItem { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public bool HasFiles { get; set; }
        public IEnumerable<string> AttachFiles { get; set; }

        public AuthorizedOrder() { }
    }
}
