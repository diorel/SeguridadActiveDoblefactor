using BanorteXXISecurity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using MethodResponse;
using BanorteXXISecurity.Business;
using BanorteXXISecurity.Helpers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BanorteXXISecurity.Controllers
{
    [ApiController]
    public class AccessADController : ControllerBase
    {
        private readonly ILogger _logger;

        public AccessADController(ILogger<AccessADController> logger)
        {
            _logger = logger;
        }

        //Internet
        [Route("BanorteXXISecurity/V1/1NT/LoginUsuario")]
        [HttpPost]
        public MethodResponse<ActiveResponse> validaUsuarioIntranet(Credential user) {
            var Response = new MethodResponse<ActiveResponse>();

            try
            {
                //var hola = 3;
                //var hola2 = 0;
                //var a = hola / hola2;

                Response = Login.loginUsuario(user, 1);
            } catch(Exception ex) {
                Response.Code = 0;
                Response.Message = "Ocurrió un error";

                string pars = JsonSerializer.Serialize(user);

                try {
                    EjecucionSP.LogErrores("AccessADController", "validaUsuarioIntranet", pars, ex.Message);
                } catch (Exception exc) {
                    _logger.Log(LogLevel.Error, exc, exc.Message);
                }
            }
            
            return Response;
        }

        //Intranet
        [Route("BanorteXXISecurity/V1/2NT/LoginUsuario")]
        [HttpPost]
        public MethodResponse<ActiveResponse> validaUsuarioInternet(Credential user)
        {
            var Response = new MethodResponse<ActiveResponse>();

            try {
                Response = Login.loginUsuario(user, 2);
            } catch (Exception ex) {
                Response.Code = 0;
                Response.Message = "Ocurrió un error";

                string pars = JsonSerializer.Serialize(user);

                try {
                    EjecucionSP.LogErrores("AccessADController", "validaUsuarioInternet", pars, ex.Message);
                } catch (Exception exc) {
                    _logger.Log(LogLevel.Error, exc, exc.Message);
                }
            }

            return Response;
        }

        [Route("BanorteXXISecurity/V1/ValidaCodigo")]
        [HttpPost]
        public ReponseApi validaCodigo(DataValidateGoogle par) {
            ReponseApi res = new ReponseApi();
            
            res.Codigo = 0;
            res.Mensaje = "";
            res.Respuesta = null;

            try {
                if (string.IsNullOrEmpty(par.Usuario)) {
                    res.Mensaje = "El Usuario no puede ser nulo";
                } else if (string.IsNullOrEmpty(par.App)) {
                    res.Mensaje = "La App no puede ser nula";
                } else if (string.IsNullOrEmpty(par.Codigo)) {
                    res.Mensaje = "El Codigo no puede ser nulo";
                } else {
                    res = DobleFactor.ValidaCodigoDobleFactor(par);
                }
            } catch (Exception ex) {
                string pars = JsonSerializer.Serialize(par);

                try {
                    EjecucionSP.LogErrores("AccessADController", "validaCodigo", pars, ex.Message);
                } catch (Exception exc) {
                    _logger.Log(LogLevel.Error, exc, exc.Message);
                }
            }

            return res;
        }
    }
}
