using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades.DTO
{
    public class CategoriaDTO
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = null!;
        public int CantidadProductos { get; set; }
    }
}
