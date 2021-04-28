using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BanorteXXISecurity.Models;
using BanorteXXISecurity.Helpers;
using Google.Authenticator;
using BanorteXXISecurity.Business;

namespace BanorteXXISecurity.Business
{
    public class DobleFactor {
        public static ReponseApi ObtenerCodigoConfig(string app, string apellidos, string nombre, string usuario, string celular, string email)
        {
            ReponseApi res = new ReponseApi();

            try
            {
                TwoFactorAuthenticator autenticador = new TwoFactorAuthenticator();
                ResponseQRGoogle resGoogle = new ResponseQRGoogle();
                var key = TwoStepsAuthenticator.Authenticator.GenerateKey();
                var setupInfo = autenticador.GenerateSetupCode(app, email, key, false, 4);
                var codigoRecuperacion = TwoStepsAuthenticator.CounterAuthenticator.GenerateKey();

                resGoogle.QRImagen = setupInfo.QrCodeSetupImageUrl;
                resGoogle.LlaveSecreta = key;
                resGoogle.CodigoManual = setupInfo.ManualEntryKey;

                string idUsuario;
                idUsuario = EjecucionSP.InsertaUsuario(apellidos, nombre, usuario, celular, email, app, key, setupInfo.ManualEntryKey, codigoRecuperacion);

                string cuerpoCorreo = "<h1>Código de Recuperación</h1><br>" + codigoRecuperacion;

                Correo.EnviarMensaje("sygno.mmadrigal@proveedores21b.com", "Configuración de Doble Factor para " + app, cuerpoCorreo);

                //resGoogle.IDUsuario = Int32.Parse(idUsuario);

                res.Codigo = 1;
                res.Mensaje = idUsuario;
                res.Respuesta = resGoogle;
            }
            catch (Exception ex)
            {
                res.Codigo = 0;
                res.Mensaje = ex.Message;
            }

            return res;
        }

        public static ReponseApi ReconfiguraToken(string app, string usuario, string email)
        {
            ReponseApi res = new ReponseApi();

            try
            {
                TwoFactorAuthenticator autenticador = new TwoFactorAuthenticator();
                ResponseQRGoogle resGoogle = new ResponseQRGoogle();
                var key = TwoStepsAuthenticator.Authenticator.GenerateKey();
                var setupInfo = autenticador.GenerateSetupCode(app, email, key, false, 4);
                var codigoRecuperacion = TwoStepsAuthenticator.CounterAuthenticator.GenerateKey();

                resGoogle.QRImagen = setupInfo.QrCodeSetupImageUrl;
                resGoogle.LlaveSecreta = key;
                resGoogle.CodigoManual = setupInfo.ManualEntryKey;

                string idUsuario;
                idUsuario = EjecucionSP.ActualizaToken(usuario, app, key, setupInfo.ManualEntryKey, codigoRecuperacion);

                string cuerpoCorreo = "<h1>Código de Recuperación</h1><br>" + codigoRecuperacion;

                Correo.EnviarMensaje("sygno.mmadrigal@proveedores21b.com", "Configuración de Doble Factor para " + app, cuerpoCorreo);

                res.Codigo = 1;
                res.Mensaje = idUsuario;
                res.Respuesta = resGoogle;
            }
            catch (Exception ex)
            {
                res.Codigo = 0;
                res.Mensaje = ex.Message;
            }

            return res;
        }

        public static ReponseApi ValidaCodigoDobleFactor(DataValidateGoogle par) {
            ReponseApi res = new ReponseApi();

            res.Codigo = 0;
            res.Mensaje = "";
            res.Respuesta = null;

            try {
                //Obtener Llave Token
                string LlaveToken = EjecucionSP.ObtenerLlaveToken(par.Usuario, par.App);

                TwoFactorAuthenticator autenticador = new TwoFactorAuthenticator();
                bool PinOK = autenticador.ValidateTwoFactorPIN(LlaveToken, par.Codigo);

                if (PinOK != true)
                {
                    res.Mensaje = "El Código ingresado no es correcto";
                }
                else
                {
                    res.Codigo = 1;
                    res.Mensaje = "Código Válido";

                    ResponseValidateCode vdCode = new ResponseValidateCode();

                    vdCode.LlaveDesbloqueo = EjecucionSP.ActualizaEstatusDobleFactor(par.Usuario, par.App, "172.15.20.1");
                    res.Respuesta = vdCode;
                }
            }
            catch (Exception ex)
            {
                //Log Error
                //_logger.Log(LogLevel.Error, ex, ex.Message);
            }

            return res;
        }
    }
}
