using CapaData.Configuracion;
using CapaData.Interfaaces;
using CapaEntidades;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace CapaData.Implementacion
{
    public class RolUsuarioRepositorio : IRolUsuarioRepositorio
    {
        private readonly ConnectionStrings con;

        public RolUsuarioRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }
        public async Task<List<RolUsuario>> Lista()
        {
            //List<Categoria> lista = [];
            List<RolUsuario> lista = [];

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_listaRolUsuario", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new RolUsuario()
                    {
                        IdRolUsuario = Convert.ToInt32(dr["IdRolUsuario"]),
                        Nombre = dr["Nombre"].ToString()
                    });
                }
            }
            return lista;
        }
    }
}
