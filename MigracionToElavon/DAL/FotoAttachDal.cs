using Microsoft.Extensions.Configuration;
using MigracionToElavon.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MigracionToElavon.DAL
{
    public class FotoAttachDal
    {
        private string _connectionString;

        public FotoAttachDal(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("MIC");
        }

        public List<FotoAttachModel> GetList()
        {
            var lista = new List<FotoAttachModel>();
            try {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT BD_FOTO_AR.ID_AR, " +
                        "BD_AR.NO_AR, BD_ATTACH.SYSTEM_FILENAME " +
                        "FROM BD_FOTO_AR " +
                        "INNER JOIN BD_AR ON BD_AR.ID_AR = BD_FOTO_AR.ID_AR " +
                        "INNER JOIN BD_ATTACH ON BD_FOTO_AR.ID_ATTACH = BD_ATTACH.ID_ATTACH " +
                        "WHERE BD_AR.ID_CLIENTE=43 AND STATUS = 'PROCESADO' AND ID_STATUS_AR <> 1", con);
                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        lista.Add(new FotoAttachModel
                        {
                            idar = Convert.ToInt32(rdr[0]),
                            noar = rdr[1].ToString(),
                            archivo = rdr[2].ToString()
                        });
                    }
                }
            }
            catch (Exception ex){
                throw ex;
            }

            return lista;
        }

        public void insertDatos(string archivo, string noar, int idar, string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO BD_ENVIO_IMAGENES_APLICACION " +
                        "VALUES("+idar+",'"+noar+"','"+archivo+"',GETDATE(),'"+msg+"')",con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
