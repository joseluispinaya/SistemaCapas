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
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly ConnectionStrings con;

        public ProductoRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<bool> Editar(Producto objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_ModificarProducto", conexion);
                cmd.Parameters.AddWithValue("@IdProducto", objeto.IdProducto);
                cmd.Parameters.AddWithValue("@IdCategoria", objeto.Categoria!.IdCategoria);
                cmd.Parameters.AddWithValue("@ImagenPro", objeto.ImagenPro);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", objeto.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioCompra", objeto.PrecioCompra);
                cmd.Parameters.AddWithValue("@PrecioVenta", objeto.PrecioVenta);
                cmd.Parameters.AddWithValue("@Cantidad", objeto.Cantidad);
                //cmd.Parameters.AddWithValue("@Activo", objeto.Activo);

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

        public async Task<bool> Guardar(Producto objeto)
        {
            bool rpta = false;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_RegistrarProducto", conexion);
                cmd.Parameters.AddWithValue("@IdCategoria", objeto.Categoria!.IdCategoria);
                cmd.Parameters.AddWithValue("@ImagenPro", objeto.ImagenPro);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", objeto.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioCompra", objeto.PrecioCompra);
                cmd.Parameters.AddWithValue("@PrecioVenta", objeto.PrecioVenta);
                cmd.Parameters.AddWithValue("@Cantidad", objeto.Cantidad);
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

        public async Task<List<Producto>> Lista()
        {
            //List<Categoria> lista = [];
            List<Producto> lista = [];

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new("usp_listaProductos", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    lista.Add(new Producto()
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        ImagenPro = dr["ImagenPro"].ToString(),
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString(),
                        PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"]),
                        PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        Activo = Convert.ToBoolean(dr["Activo"]),
                        Categoria = new Categoria()
                        {
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                            Nombre = dr["NombreCategoria"].ToString()!
                        }
                    });
                }
            }
            return lista;
        }

    }
}
