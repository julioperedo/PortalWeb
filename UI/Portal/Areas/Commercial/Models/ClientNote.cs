using BEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEK = BEntities.Kbytes;

namespace Portal.Areas.Commercial.Models
{
    public class ClientNote
    {
        public string Subsidiary { get; set; }

        public long Number { get; set; }

        public string CardCode { get; set; }

        public DateTime NoteDate { get; set; }

        public decimal Amount { get; set; }

        public string Terms { get; set; }

        public long IdStatus { get; set; }

        public decimal AcceleratorPeriod { get; set; }

        public List<ClientNoteItem> Items { get; set; }

        public BEK.ClientNote ToEntity(long? LogUser, DateTime? LogDate, StatusType Action = StatusType.NoAction)
        {
            BEK.ClientNote note = new BEK.ClientNote
            {
                Subsidiary = Subsidiary,
                Number = Number,
                CardCode = CardCode,
                Date = NoteDate,
                Amount = Amount,
                Terms = Terms,
                Enabled = true,
                LogUser = LogUser ?? 0,
                LogDate = LogDate ?? DateTime.Now,
                StatusType = Action,
                ListClientNoteDetails = Items?.Select(x => x.ToEntity(LogUser, LogDate, Action)).ToList() ?? new List<BEK.ClientNoteDetail>(),
                ListStatusDetails = new List<BEK.StatusDetail>()
            };

            return note;
        }
    }

    public class ClientNoteItem
    {
        public long IdProduct { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public int AcceleratedQuantity { get; set; }

        public decimal AcceleratedTotal { get; set; }

        public decimal Accelerator { get; set; }

        public decimal ExtraPoints { get; set; }

        public long IdLot { get; set; }

        public int RemainQuantity { get; set; }

        public bool Enabled { get; set; }

        public BEK.ClientNoteDetail ToEntity(long? LogUser, DateTime? LogDate, StatusType Action = StatusType.NoAction)
        {
            BEK.ClientNoteDetail item = new BEK.ClientNoteDetail
            {
                IdProduct = IdProduct,
                Quantity = Quantity,
                Total = Total,
                AcceleratedQuantity = AcceleratedQuantity,
                AcceleratedTotal = AcceleratedTotal,
                Accelerator = Accelerator,
                ExtraPoints = ExtraPoints,
                LogUser = LogUser ?? 0,
                LogDate = LogDate ?? DateTime.Now,
                StatusType = Action
            };
            return item;
        }

    }
}
