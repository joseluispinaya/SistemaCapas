using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Interfaaces
{
    public interface IProductoRepositorio
    {
        Task<List<Producto>> Lista();
        Task<bool> Guardar(Producto objeto);
        Task<bool> Editar(Producto objeto);
        //Task<bool> Editar(Categoria objeto);
    }
}
