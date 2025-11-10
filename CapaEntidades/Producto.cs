using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string? ImagenPro { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public Categoria? Categoria { get; set; }
        public int Cantidad { get; set; }
        public bool? Activo { get; set; }
        public string PhotoFull => string.IsNullOrEmpty(ImagenPro) ? "/images/bot1.png" : ImagenPro;
    }
}
