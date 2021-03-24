using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BanorteXXISecurity.Data;
using BanorteXXISecurity.Models;

namespace BanorteXXISecurity.Business
{
    public class EjecucionSP
    {
        public static bool RequiereDobleFactor(string app, int area, int perfil) {
            bool res;

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();
            ParametrosSP par4 = new ParametrosSP();

            par1.Nombre = "P_APP";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = app;

            pars.Add(par1);

            par2.Nombre = "P_AREA";
            par2.Direccion = "in";
            par2.Tipo = "number";
            par2.Valor = area;

            pars.Add(par2);

            par3.Nombre = "P_PERFIL";
            par3.Direccion = "in";
            par3.Tipo = "number";
            par3.Valor = perfil;

            pars.Add(par3);

            par4.Nombre = "P_RESULTADO";
            par4.Direccion = "out";
            par4.Tipo = "varchar2";
            par4.Valor = "";

            pars.Add(par4);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_REQUIERE_DOBLE_FACTOR", pars) == "1";

            return res;
        }

        public static int UsuarioConfigurado(string usuario, string app) {
            int res;

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();

            par1.Nombre = "P_USUARIO";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = usuario;

            pars.Add(par1);

            par2.Nombre = "P_APP";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = app;

            pars.Add(par2);

            par3.Nombre = "P_RESULTADO";
            par3.Direccion = "out";
            par3.Tipo = "varchar2";
            par3.Valor = "";

            pars.Add(par3);

            try {
                res = Int32.Parse(Dal.EjecutaSP("PKG_SECURITY.SP_VALIDA_USUARIO_TOKEN", pars));
            } catch(Exception ex) {
                //Guardar en log el error
                res = 0;
            }
            

            return res;
        }

        public static string InsertaUsuario(string apellidos, string nombre, string usuario, string celular, string email, 
                                            string app, string llave, string config) {
            string res = "";

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();
            ParametrosSP par4 = new ParametrosSP();
            ParametrosSP par5 = new ParametrosSP();
            ParametrosSP par6 = new ParametrosSP();
            ParametrosSP par7 = new ParametrosSP();
            ParametrosSP par8 = new ParametrosSP();
            ParametrosSP par9 = new ParametrosSP();

            par1.Nombre = "P_APELLIDOS";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = apellidos;

            pars.Add(par1);

            par2.Nombre = "P_NOMBRE";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = nombre;

            pars.Add(par2);

            par3.Nombre = "P_USUARIO";
            par3.Direccion = "in";
            par3.Tipo = "varchar2";
            par3.Valor = usuario;

            pars.Add(par3);

            par4.Nombre = "P_CELULAR";
            par4.Direccion = "in";
            par4.Tipo = "varchar2";
            par4.Valor = celular;

            pars.Add(par4);

            par5.Nombre = "P_EMAIL";
            par5.Direccion = "in";
            par5.Tipo = "varchar2";
            par5.Valor = email;

            pars.Add(par5);

            par6.Nombre = "P_APP";
            par6.Direccion = "in";
            par6.Tipo = "varchar2";
            par6.Valor = app;

            pars.Add(par6);

            par7.Nombre = "P_LLAVE";
            par7.Direccion = "in";
            par7.Tipo = "varchar2";
            par7.Valor = llave;

            pars.Add(par7);

            par8.Nombre = "P_CONFIG";
            par8.Direccion = "in";
            par8.Tipo = "varchar2";
            par8.Valor = config;

            pars.Add(par8);

            par9.Nombre = "P_RESULTADO";
            par9.Direccion = "out";
            par9.Tipo = "varchar2";
            par9.Valor = "";

            pars.Add(par9);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_INSERTA_USUARIO", pars);

            return res;
        }

        public static string ActualizaLlaveDesbloqueo(int usuario, string app, string llave) {
            string res = "";

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();
            ParametrosSP par4 = new ParametrosSP();

            par1.Nombre = "P_USUARIO";
            par1.Direccion = "in";
            par1.Tipo = "number";
            par1.Valor = usuario;

            pars.Add(par1);

            par2.Nombre = "P_APP";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = app;

            pars.Add(par2);

            par3.Nombre = "P_LLAVE";
            par3.Direccion = "in";
            par3.Tipo = "varchar2";
            par3.Valor = llave;

            pars.Add(par3);

            par4.Nombre = "P_RESULTADO";
            par4.Direccion = "out";
            par4.Tipo = "varchar2";
            par4.Valor = "";

            pars.Add(par4);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_ACTUALIZA_LLAVE", pars);

            return res;
        }

        public static string ActualizaEstatusDobleFactor(string usuario, string app) {
            string res = "";

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();

            par1.Nombre = "P_USUARIO";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = usuario;

            pars.Add(par1);

            par2.Nombre = "P_APP";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = app;

            pars.Add(par2);

            par3.Nombre = "P_RESULTADO";
            par3.Direccion = "out";
            par3.Tipo = "varchar2";
            par3.Valor = "";

            pars.Add(par3);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_ACTUALIZA_ESTATUS_DOBLE_FACTOR", pars);

            return res;
        }

        public static string ObtenerLlaveToken(string usuario, string app)
        {
            string res = "";

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();

            par1.Nombre = "P_USUARIO";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = usuario;

            pars.Add(par1);

            par2.Nombre = "P_APP";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = app;

            pars.Add(par2);

            par3.Nombre = "P_RESULTADO";
            par3.Direccion = "out";
            par3.Tipo = "varchar2";
            par3.Valor = "";

            pars.Add(par3);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_OBTIENE_LLAVE", pars);

            return res;
        }

        public static string BitacoraAcceso(string usuario, string app)
        {
            string res = "";

            List<dynamic> pars = new List<dynamic>();

            ParametrosSP par1 = new ParametrosSP();
            ParametrosSP par2 = new ParametrosSP();
            ParametrosSP par3 = new ParametrosSP();

            par1.Nombre = "P_USUARIO";
            par1.Direccion = "in";
            par1.Tipo = "varchar2";
            par1.Valor = usuario;

            pars.Add(par1);

            par2.Nombre = "P_APP";
            par2.Direccion = "in";
            par2.Tipo = "varchar2";
            par2.Valor = app;

            pars.Add(par2);

            par3.Nombre = "P_RESULTADO";
            par3.Direccion = "out";
            par3.Tipo = "varchar2";
            par3.Valor = "";

            pars.Add(par3);

            res = Dal.EjecutaSP("PKG_SECURITY.SP_BITACORA_ACCESO", pars);

            return res;
        }
    }
}
