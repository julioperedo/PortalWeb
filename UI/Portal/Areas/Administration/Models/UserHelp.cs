using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Administration.Models
{
    public class UserHelp
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Value { get; set; }


        public UserHelp(BEntities.Base.UserHelp Item)
        {
            Id = Item.Id;
            Title = Item.Title;
            Value = Item.Value;
        }

        public BEntities.Base.UserHelp ToEntity(long UserId = 0)
        {
            BEntities.Base.UserHelp item = new BEntities.Base.UserHelp { Id = Id, Title = Title, Value = Value, LogDate = DateTime.Now, LogUser = UserId };
            return item;
        }
    }
}
