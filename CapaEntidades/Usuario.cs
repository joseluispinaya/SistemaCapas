using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NroCi { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public RolUsuario RolUsuario { get; set; } = null!;
        public bool? Activo { get; set; }
        public string FechaCreacion { get; set; } = null!;
    }
}
