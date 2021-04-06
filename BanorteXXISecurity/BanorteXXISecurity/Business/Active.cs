using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using MethodResponse;
using ActiveDs;
using BanorteXXISecurity.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.InteropServices;

namespace BanorteXXISecurity.Business
{
    public class Active
    {
        private string MasterUser;

        public Active()
        {
            var configuration = GetConfiguration();
            MasterUser = configuration.GetSection("Configuraciones").GetSection("MasterUser").Value;
        }


        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }



        public MethodResponse<DirectoryEntry> ValidateCredentials(string domain, string username, string password, string app, int tipo)
        {
            var Response = new MethodResponse<DirectoryEntry>();
            Response.Code = 1;

            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
               
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);
                var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                ActiveDs.IADsUser native = (ActiveDs.IADsUser)dirEntry.NativeObject;

                DateTime passwordExpirationDate = native.PasswordExpirationDate;
                var intentosFallidos = 3 - native.BadLoginCount;

                var diasFaltantes = DateTime.Today - passwordExpirationDate;

                int cod;
                cod = (int)dirEntry.Properties["useraccountcontrol"].Value;

                if (cod == 512)
                {
                    if (passwordExpirationDate < DateTime.Now)
                    {
                        Response.Code = 0;
                        Response.Message = "Tu contraseña ha expirado";

                        return Response;
                    }

                    var diasRestantes = Math.Floor((passwordExpirationDate - DateTime.Now).TotalDays);
                    var horasRestantes = Math.Floor((passwordExpirationDate - DateTime.Now).TotalHours);
                    var minutosRestantes = Math.Ceiling((passwordExpirationDate - DateTime.Now).TotalMinutes);

                    //1 - Intranet || 2 - Público
                    if(tipo == 1)
                    {
                        if (diasRestantes <= 7 && diasRestantes > 0)
                            Response.Message = "Tu contraseña expirará en " + diasRestantes + (diasRestantes == 1 ? " día" : " días");
                        else if (diasRestantes == 0)
                        {
                            if (horasRestantes > 0)
                                Response.Message = "Tu contraseña expirará en " + horasRestantes + (horasRestantes == 1 ? " hora" : " horas");
                            else
                                Response.Message = "Tu contraseña expirará en " + minutosRestantes + (minutosRestantes == 1 ? " minuto" : " minutos");
                        }
                    } else {
                        Response.Message = "Tu contraseña está próxima a expirar";
                    }

                    
                }

                if (cod == 514 || cod == 66050)
                {
                    Response.Code = 0;
                    Response.Message = "La cuenta " + username + " está inactiva";
                }
                else
                {
                    if (user != null)
                    {
                        if (user.IsAccountLockedOut())
                        {
                            Response.Code = 0;

                            if(tipo == 1 )
                            {
                                Response.Message = "La cuenta " + username + " está bloqueada, se desbloqueará en 30 minutos";
                            } else
                            {
                                Response.Message = "La cuenta " + username + " está bloqueada";
                            }
                            
                        }
                        else
                        {
                            if (validaApp(app, username))
                            {
                                DirectoryEntry de = new DirectoryEntry(domain, username, password);
                                DirectorySearcher ds = new DirectorySearcher(de);
                                string qry = string.Format("(&(objectCategory=person)(sAMAccountName={0}))", username);
                                ds.Filter = qry;
                                try
                                {
                                    //object obj = de.NativeObject;
                                    SearchResult sr = ds.FindOne();

                                    if (sr != null)
                                    {
                                        de = sr.GetDirectoryEntry();
                                        Response.Result = de;
                                    }
                                }
                                catch (DirectoryServicesCOMException)
                                {
                                    Response.Code = 0;

                                    intentosFallidos--;

                                    if(tipo == 1)
                                    {
                                        Response.Message = "La contraseña es incorrecta. Solo te " + (intentosFallidos > 1 ? "quedan " : "queda ") + intentosFallidos + (intentosFallidos > 1 ? " intentos" : " intento");
                                    } else
                                    {
                                        Response.Message = "El usuario y/o la contraseña son incorrectos";
                                    }
                                }
                                catch (Exception)
                                {
                                    Response.Code = 0;
                                    Response.Message = "Ocurrió un error";

                                   // Dal.LogError(app, username, "Active.ValidateCredentials: " + ce.Message);
                                }
                            }
                            else
                            {
                                Response.Code = 0;
                                Response.Message = "El código de aplicación no existe";
                            }
                        }
                    }
                    else
                    {
                        Response.Code = 0;

                        if(tipo == 1) {
                            Response.Message = string.Format("La cuenta " + username + " no existe");
                        } else {
                            Response.Message = "El usuario y/o la contraseña son incorrectos";
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Response.Code = 0;

                if (tipo == 1)
                {
                    Response.Message = string.Format("La cuenta " + username + " no existe");
                }
                else
                {
                    Response.Message = "El usuario y/o la contraseña son incorrectos";
                }
            }
            return Response;
        }

        private Boolean validaApp(string app, string userName)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, MasterUser);

                var dirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                string apps = dirEntry.Properties["pager"].Value.ToString();

                return apps.Contains(app);
            }
            catch (Exception)
            {
               // Dal.LogError(app, userName, "Active.validaApp: " + ex.Message);
                return true;
            }

        }

    }
}
