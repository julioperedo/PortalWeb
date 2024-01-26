using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Visits.Models {
    public class Visit {
        public long Id { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "*")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Visitante")]
        public long VisitorId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Staff")]
        public long StaffId { get; set; }

        [Display(Name = "Motivo")]
        public string ReasonVisit { get; set; }

        [Required(ErrorMessage = "*")]
        public long IdReason { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "No. Tarjeta")]
        public long SecurityCardId { get; set; }

        [Display(Name = "Comentarios")]
        public string Commentaries { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        [Display(Name = "Descripción Cliente")]
        public string ClientDescription { get; set; }

        public string ClientName { get; set; }

        public string VisitorName { get; set; }

        public string StaffName { get; set; }

        [Display(Name = "F. Ingreso")]
        public string Initialdate { get; set; }

        [Display(Name = "F. Salida")]
        public string Finaldate { get; set; }

        public bool Editable { get; set; }

        public Visit() { }

        public Visit(BEntities.Visits.VisitReception Item) {
            Id = Item.Id;
            CardCode = Item.CardCode;
            VisitorId = Item.VisitorId;
            StaffId = Item.StaffId;
            IdReason = Item.IdReason ?? 0;
            ReasonVisit = Item.ReasonVisit;
            SecurityCardId = Item.SecurityCardId;
            Commentaries = Item.Commentaries;
            ClientDescription = Item.ClientDescription;
            Editable = Item.Editable;

            VisitorName = Item.Visitor?.FullName ?? "";
            StaffName = Item.Staff?.FullName ?? "";

            Initialdate = Item.InitialDate.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public BEntities.Visits.VisitReception ToVisitReception() {
            BEntities.Visits.VisitReception Item = new BEntities.Visits.VisitReception {
                Id = Id, CardCode = CardCode, VisitorId = VisitorId, StaffId = StaffId, IdReason = IdReason, ReasonVisit = ReasonVisit, SecurityCardId = SecurityCardId, Commentaries = Commentaries,
                ClientDescription = ClientDescription
            };
            return Item;
        }

        public BEntities.Visits.VisitReception ToVisitReception(BEntities.Visits.VisitReception Item) {
            Item.CardCode = CardCode;
            Item.ClientDescription = ClientDescription;
            Item.VisitorId = VisitorId;
            Item.StaffId = StaffId;
            Item.IdReason = IdReason;
            Item.ReasonVisit = ReasonVisit;
            Item.SecurityCardId = SecurityCardId;
            Item.Commentaries = Commentaries;

            return Item;
        }
    }
}
