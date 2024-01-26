using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models {
    public class LoginModel {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Empresa")]
        public string CardCode { get; set; }

        [Display(Name = "Perfil")]
        public long? IdProfile { get; set; }
    }
}
