using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Marketing.Models {
    public class PopupNotification {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Frequency { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Value { get; set; }

        public string MobileValue { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Popup { get; set; }

        public IEnumerable<long> Lines { get; set; }

        public IEnumerable<string> Clients { get; set; }

        public PopupNotification(BEntities.Base.Notification Item) {
            Id = Item.Id;
            Name = Item.Name;
            Description = Item.Description;
            InitialDate = Item.InitialDate;
            FinalDate = Item.FinalDate;
            Enabled = Item.Enabled;
            Frequency = Item.Frequency;
            Value = Item.Value;
            Popup = Item.Popup;
            MobileValue = Item.MobileValue;
            Lines = Item.ListNotificationDetails?.Select(x => x.IdLine);
            Clients = Item.ListNotificationClients?.Select(x => x.CardCode);
        }
    }

    public class NotificationFilter {
        public string Name { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
        public string State { get; set; }
    }
}
