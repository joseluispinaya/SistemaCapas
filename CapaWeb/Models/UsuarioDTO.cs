namespace CapaWeb.Models
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }          // solo para editar
        public string NroCi { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public int IdRolUsuario { get; set; }
    }
}
