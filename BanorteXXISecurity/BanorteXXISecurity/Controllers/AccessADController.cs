using BanorteXXISecurity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MethodResponse;
using BanorteXXISecurity.Business;
using BanorteXXISecurity.Helpers;
using BanorteXXISecurity.Data;

namespace BanorteXXISecurity.Controllers
{

    /// <summary>
    /// comentario Raul Cortes
    /// </summary>

    [ApiController]
    public class AccessADController : ControllerBase
    {
        [Route("BanorteXXISecurity/V1/LoginUsuario")]
        [HttpPost]
        public MethodResponse<ActiveResponse> validaLogin(Credential user) {
            Validate ActiveUser = new Validate();

            GenerateToken NewToken = new GenerateToken();
            var Response = new MethodResponse<ActiveResponse>();
            ActiveResponse objectActive = new ActiveResponse();
            int expiraToken;

            BodyToken Token = new BodyToken();

            if (string.IsNullOrEmpty(user.User))
            {
                Response.Code = 0;
                Response.Message = string.Format("El usuario no puede ser nulo");
            }
            else if (string.IsNullOrEmpty(user.Password))
            {
                Response.Code = 0;
                Response.Message = string.Format("La contraseña no puede ser nula");
            }
            else if (string.IsNullOrEmpty(user.App))
            {
                Response.Code = 0;
                Response.Message = string.Format("El código de aplicación no puede ser nulo");
            }
            else
            {
                try
                {
                    //var hola = 3;
                    //var hola2 = 0;
                    //var a = hola / hola2;

                    expiraToken = user.ExpiracionToken == 0 ? 60 : user.ExpiracionToken;

                    var MetaDatos = ActiveUser.GetUserActive(user.User, user.Password, user.App, expiraToken);
                    if (MetaDatos.Code != 0) {
                        //Requiere doble factor
                        bool requiere;
                        requiere = EjecucionSP.RequiereDobleFactor(user.App, MetaDatos.Result.Permisos.Area, MetaDatos.Result.Permisos.Perfil);

                        if(requiere) {
                            int config;
                            //Valida si el usuario tiene configurado el token para doble factor
                            config = EjecucionSP.UsuarioConfigurado(user.User, user.App);

                            if(config > 0) {
                                //Se devuelve el response encriptado
                                string llaveDesbloqueo = Encriptacion.GeneraLlave(config);

                                Response.Code = 3;
                                Response.Result = Encriptacion.EncriptaResponse(MetaDatos.Result, Encriptacion.GeneraLlave(config));

                                EjecucionSP.ActualizaLlaveDesbloqueo(config, user.App, llaveDesbloqueo);
                            } else {
                                //Si no está configurado se devuelve el código QR de configuración
                                ResponseQRGoogle codQR = new ResponseQRGoogle();

                                codQR = DobleFactor.ObtenerCodigoConfig(user.App, MetaDatos.Result.Apellidos, 
                                    MetaDatos.Result.Nombre, user.User, MetaDatos.Result.Telefono, MetaDatos.Result.Email).Respuesta;

                                string llaveDesbloqueo = Encriptacion.GeneraLlave(codQR.IDUsuario);

                                Response.Code = 2;
                                Response.Result = Encriptacion.EncriptaResponse(MetaDatos.Result, llaveDesbloqueo);
                                Response.Result.CodigoQR = codQR;

                                EjecucionSP.ActualizaLlaveDesbloqueo(codQR.IDUsuario, user.App, llaveDesbloqueo);
                            }
                        } else {
                            // todo paso bien 
                            Response.Code = 1;
                            Response.Result = MetaDatos.Result;

                            // genera token de seguridad 
                            Token = NewToken.CreateToken(user.User, user.ExpiracionToken);

                            //Inserta registro en bitácora
                            EjecucionSP.BitacoraAcceso(user.User, user.App);

                            // se añade al objeto token sus propiedades 
                            //InformationToken infoToken = new InformationToken();

                            //infoToken.Token = Token.TokenCreated;
                            //infoToken.ExpiracionToken = Token.TimeToken;

                            //Response.Result.InformacionToken.Add(infoToken);
                        }   
                    }
                    else
                    {
                        Response.Code = 0;
                        Response.Message = MetaDatos.Message;
                    }
                }
                catch (Exception ex)
                {
                    Response.Code = 0;
                    Response.Message = "Ocurrió un error";

                    //Dal.LogError(user.App, user.User, "AccessADController.validaLogin: " + exception.Message);
                }
            }

            return Response;
        }

        [Route("BanorteXXISecurity/V1/ValidaCodigo")]
        [HttpPost]
        public ReponseApi validaLogin(DataValidateGoogle par) {
            ReponseApi res = new ReponseApi();

            res.Codigo = 0;
            res.Mensaje = "";
            res.Respuesta = null;

            try {
                if (string.IsNullOrEmpty(par.Usuario)) {
                    res.Mensaje = "El Usuario no puede ser nulo";
                } else if (string.IsNullOrEmpty(par.App))
                {
                    res.Mensaje = "La App no puede ser nula";
                }
                else if (string.IsNullOrEmpty(par.Codigo))
                {
                    res.Mensaje = "El Codigo no puede ser nulo";
                }
                else
                {
                    res = DobleFactor.ValidaCodigoDobleFactor(par);
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
