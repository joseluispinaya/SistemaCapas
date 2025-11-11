using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Interfaaces
{
    public interface IRolUsuarioRepositorio
    {
        Task<List<RolUsuario>> Lista();
    }
}
