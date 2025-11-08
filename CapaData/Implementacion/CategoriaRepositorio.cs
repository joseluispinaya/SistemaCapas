using CapaData.Configuracion;
using CapaData.Interfaaces;
using CapaEntidades;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Implementacion
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ConnectionStrings con;

        public CategoriaRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<bool> Editar(Categoria objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("usp_ModificarCategoria", conexion);
                cmd.Parameters.AddWithValue("@IdCategoria", objeto.IdCategoria);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Activo", objeto.Activo);

                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    rpta = Convert.ToBoolean(cmd.Parameters["@Resultado"].Value);
                }
                catch
                {
                    rpta = false;
                }

            }
            return rpta;
        }

        public async Task<bool> Guardar(Categoria objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_RegistrarCategoria", conexion);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    rpta = Convert.ToBoolean(cmd.Parameters["@Resultado"].Value);
                }
                catch
                {
                    rpta = false;
                }

            }
            return rpta;
        }

        public async Task<List<Categoria>> Lista()
        {
            //List<Categoria> lista = [];
            List<Categoria> lista = [];

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_listaCategoria", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Categoria()
                    {
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        Nombre = dr["Nombre"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"])
                    });
                }
            }
            return lista;
        }
    }
}
