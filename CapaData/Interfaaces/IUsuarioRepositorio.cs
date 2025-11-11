using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Interfaaces
{
    public interface IUsuarioRepositorio
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Login(string Correo, string Clave);
        Task<Usuario?> LoginNuevo(string Correo, string Clave);
        //Task<Usuario?> LoginNuevo(string Correo, string Clave);
        Task<bool> Guardar(Usuario objeto);
        Task<bool> Editar(Usuario objeto);
        Task<bool> CambioEstado(int Id, bool Activo);
    }
}
