using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using BanorteXXISecurity.Models;

namespace BanorteXXISecurity.Data
{
    public class Dal
    {
        public static string EjecutaSP(string nombreSP, List<dynamic> parametros) {
            string res = "";

            try
            {
                string connectionString = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 15.128.25.150)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = solidaD))); User ID=SIAJXXIB; Password=Siaj$0107";

                using (OracleConnection connection = new OracleConnection(connectionString)) {
                    connection.Open();

                    OracleDataAdapter da = new OracleDataAdapter();
                    OracleCommand cmd = new OracleCommand(nombreSP, connection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (dynamic par in parametros) {
                        if (par.Direccion == "in") {
                            cmd.Parameters.Add(par.Nombre, ObtieneTipoPar(par.Tipo), par.Valor, ObtieneDireccionPar(par.Direccion));
                        } else {
                            cmd.Parameters.Add(par.Nombre, ObtieneTipoPar(par.Tipo), 4000, "", ObtieneDireccionPar(par.Direccion));
                        }
                    }

                    cmd.ExecuteNonQuery();

                    res = cmd.Parameters["P_RESULTADO"].Value.ToString();
                }
            } catch(Exception ex) {
                throw ex;
            }
            
            return res;
        }

        public static OracleDbType ObtieneTipoPar(string tipo) {
            OracleDbType par = new OracleDbType();

            switch(tipo){
                case "varchar2": par = OracleDbType.Varchar2;
                    break;

                case "number": par = OracleDbType.Int32;
                    break;

                case "date": par = OracleDbType.Date;
                    break;

                case "char": par = OracleDbType.Char;
                    break;

                case "long": par = OracleDbType.Long;
                    break;

                default:
                    par = OracleDbType.Varchar2;
                    break;
            }

            return par;
        }

        public static ParameterDirection ObtieneDireccionPar(string direccion) {
            ParameterDirection dir = new ParameterDirection();

            switch(direccion) {
                case "in": dir = ParameterDirection.Input;
                    break;

                case "out": dir = ParameterDirection.Output;
                    break;

                case "return": dir = ParameterDirection.ReturnValue;
                    break;

                default: dir = ParameterDirection.Input;
                    break;
            }

            return dir;
        }
    }
}