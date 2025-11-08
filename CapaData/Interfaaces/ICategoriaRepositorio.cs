using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Interfaaces
{
    public interface ICategoriaRepositorio
    {
        Task<List<Categoria>> Lista();
        Task<bool> Guardar(Categoria objeto);
        Task<bool> Editar(Categoria objeto);
        //Task<bool> Editar(Categoria objeto);
    }
}
