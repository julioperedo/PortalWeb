using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{

    public class Payment : BEntity
    {
        #region Properties

        public string Subsidiary { get; set; }
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string State { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        //public int NumNota { get; set; }
        public decimal NotePaidAmount { get; set; }
        public decimal OnAccount { get; set; }
        public decimal Total { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal NoteTotal { get; set; }
        public decimal NotAppliedTotal { get; set; }
        public string Comments { get; set; }
        public int DueDays { get; set; }
        public int? NoteNumber { get; set; }
        public DateTime? NoteDate { get; set; }
        public string Terms { get; set; }
        public string IsDeliveryNote { get; set; }
        public string JournalComments { get; set; }

        #endregion

        #region Contructors

        public Payment() { }

        #endregion
    }

}
