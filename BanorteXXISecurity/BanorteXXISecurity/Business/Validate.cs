using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using BanorteXXISecurity.Data;
using BanorteXXISecurity.Models;
using MethodResponse;
using System.Configuration;

using Microsoft.Extensions.Configuration;
using System.IO;

namespace BanorteXXISecurity.Business
{
    public class Validate
    {

  
        private string LDAP;

        public Validate()
        {
            var configuration = GetConfiguration();
             LDAP = configuration.GetSection("Configuraciones").GetSection("LDAP_Path").Value;
        }


        public IConfigurationRoot GetConfiguration() 
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }


        public MethodResponse<ActiveResponse> GetUserActive(string usuario, string password, string app, int expiraciontoken, int tipo)
        {      
            var Response = new MethodResponse<ActiveResponse>();
            var ResponseA = new MethodResponse<DirectoryEntry>();
            Active ldap = new Active();
            GenerateToken NewToken = new GenerateToken();

            Response.Code = 1;

            try
            {
                ActiveResponse objectActive = new ActiveResponse();
                objectActive.Aplicaciones = new List<Application>();

                ResponseA = ldap.ValidateCredentials(LDAP, usuario, password, app, tipo);

                if (ResponseA.Code != 0)
                {
                    if (getField(ResponseA.Result, "pager").dato != null)
                    {
                        var arrayAccesos = getField(ResponseA.Result, "pager").dato.Split(';');
                        Permission ObjApp = new Permission();

                        foreach (string acceso in arrayAccesos)
                        {
                            Application Application = new Application();
                            if (acceso != "")
                            {
                                Application.Codigo = acceso.Substring(0, acceso.IndexOf('=')).ToString();
                                Application.AreaPerfil = acceso.Substring(acceso.IndexOf('=') + 1).ToString();

                                objectActive.Aplicaciones.Add(Application);
                                objectActive.InformacionToken = new List<InformationToken>();

                                if (Application.Codigo == app)
                                {
                                    ObjApp.Area = int.Parse(Application.AreaPerfil.Substring(0, Application.AreaPerfil.IndexOf(',')));
                                    ObjApp.Perfil = int.Parse(acceso.Substring(acceso.IndexOf(',') + 1));
                                }
                            }
                        }

                        var time = getField(ResponseA.Result, "lockoutTime").dato;

                        if (ObjApp.Area != 0 && ObjApp.Perfil != 0)
                        {
                            objectActive.Permisos = ObjApp;

                            objectActive.Nombre = getField(ResponseA.Result, "givenName").dato;
                            objectActive.Apellidos = getField(ResponseA.Result, "sn").dato;
                            objectActive.Descripcion = getField(ResponseA.Result, "description").dato;
                            objectActive.Oficina = getField(ResponseA.Result, "physicalDeliveryOfficeName").dato;
                            objectActive.Telefono = getField(ResponseA.Result, "telephoneNumber").dato;
                            objectActive.Email = getField(ResponseA.Result, "mail").dato;
                            objectActive.Direccion = getField(ResponseA.Result, "streetAddress").dato;
                            objectActive.NumNomina = getField(ResponseA.Result, "postOfficeBox").dato;
                            objectActive.Puesto = getField(ResponseA.Result, "title").dato;
                            objectActive.Departamento = getField(ResponseA.Result, "department").dato;
                            objectActive.Extension = getField(ResponseA.Result, "ipPhone").dato;
                            objectActive.NombreCompleto = getField(ResponseA.Result, "displayName").dato;
                            objectActive.NombreUsuario = getField(ResponseA.Result, "sAMAccountName").dato;
                            objectActive.Compañia = getField(ResponseA.Result, "company").dato;
                            objectActive.Notas = getField(ResponseA.Result, "info").dato;
                            //En esta parte se genera el token 
                            InformationToken Token = new InformationToken();
                            var Tokenresult = NewToken.CreateToken(usuario, expiraciontoken);
                            Token.ExpiracionToken = Tokenresult.TimeToken;
                            Token.Token = Tokenresult.TokenCreated;
                            objectActive.InformacionToken.Add(Token);

                            try
                            {
                                byte[] bt = (byte[])ResponseA.Result.Properties["thumbnailPhoto"].Value;
                                //objectActive.foto = BitConverter.ToString(bt);
                                objectActive.Foto = Convert.ToBase64String(bt);
                            }
                            catch (Exception)
                            {
                                objectActive.Foto = null;
                            }

                            Response.Result = objectActive;
                           // Dal.LogAcceso(app, usuario);
                        }
                        else
                        {
                            Response.Code = 0;
                            Response.Message = string.Format("No tienes permisos para esta aplicación");
                        }
                    }
                    else
                    {
                        Response.Code = 0;
                        Response.Message = "El usuario no tiene aplicaciones configuradas";
                    }
                }
                else
                {
                    Response.Code = ResponseA.Code;
                    Response.Message = ResponseA.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Response;
        }


        public GenericData<string> getField(DirectoryEntry de, string campo)
        {
            var Response = new GenericData<string>();
            try
            {
                Response.dato = de.InvokeGet(campo).ToString();
            }
            catch (Exception ex)
            {
                Response.dato = null;
                Console.WriteLine("Error: " + ex.Message);
            }

            return Response;
        }


    }
}
