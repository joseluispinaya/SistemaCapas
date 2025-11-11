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
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ConnectionStrings con;

        public UsuarioRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<bool> CambioEstado(int Id, bool Activo)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_ModifEstado", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@IdUsuario", Id);
                cmd.Parameters.AddWithValue("@Activo", Activo);

                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

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

        public async Task<bool> Editar(Usuario objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_ModificarUsuario", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@IdUsuario", objeto.IdUsuario);
                cmd.Parameters.AddWithValue("@NroCi", objeto.NroCi);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Clave", objeto.Clave);
                cmd.Parameters.AddWithValue("@IdRolUsuario", objeto.RolUsuario.IdRolUsuario);

                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

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

        public async Task<bool> Guardar(Usuario objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_RegistrarUsuario", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@NroCi", objeto.NroCi);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Clave", objeto.Clave);
                cmd.Parameters.AddWithValue("@IdRolUsuario", objeto.RolUsuario.IdRolUsuario);

                cmd.Parameters.Add("@Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

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

        public async Task<List<Usuario>> Lista()
        {
            //List<Categoria> lista = [];
            List<Usuario> lista = [];

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_listaUsuarios", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        NroCi = dr["NroCi"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Apellido = dr["Apellido"].ToString()!,
                        Correo = dr["Correo"].ToString()!,
                        Clave = dr["Clave"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        RolUsuario = new RolUsuario()
                        {
                            IdRolUsuario = Convert.ToInt32(dr["IdRolUsuario"]),
                            Nombre = dr["NombreRol"].ToString(),
                        },
                        FechaCreacion = dr["FechaCreacion"].ToString()!
                    });
                }
            }
            return lista;
        }

        public async Task<Usuario> Login(string Correo, string Clave)
        {
            Usuario objeto = null!;
            //Usuario objeto = new();
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_loginUsuario", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Correo", Correo);
                cmd.Parameters.AddWithValue("@Clave", Clave);

                using var dr = await cmd.ExecuteReaderAsync();
                if (await dr.ReadAsync())
                {
                    objeto = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        NroCi = dr["NroCi"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Apellido = dr["Apellido"].ToString()!,
                        Correo = dr["Correo"].ToString()!,
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        RolUsuario = new RolUsuario
                        {
                            Nombre = dr["NombreRol"].ToString(),
                        }
                    };
                }

            }
            return objeto;
        }

        public async Task<Usuario?> LoginNuevo(string correo, string clave)
        {
            Usuario? objeto = null;

            using var conexion = new SqlConnection(con.CadenaSQL);
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_loginUsuario", conexion)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Correo", correo);
            cmd.Parameters.AddWithValue("@Clave", clave);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                objeto = new Usuario()
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    NroCi = dr["NroCi"].ToString()!,
                    Nombre = dr["Nombre"].ToString()!,
                    Apellido = dr["Apellido"].ToString()!,
                    Correo = dr["Correo"].ToString()!,
                    Activo = Convert.ToBoolean(dr["Activo"]),
                    RolUsuario = new RolUsuario
                    {
                        Nombre = dr["NombreRol"].ToString()!,
                    }
                };
            }

            return objeto;
        }
    }
}
