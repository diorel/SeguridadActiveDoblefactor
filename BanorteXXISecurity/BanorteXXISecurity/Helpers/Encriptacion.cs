using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BanorteXXISecurity.Models;

namespace BanorteXXISecurity.Helpers
{
    public class Encriptacion
    {
        public static string Encriptar(string llaveEncriptar, string texto) {
            try {
                //string key = "banorteonfnoi23nro2389912837!"; //llave para encriptar datos
                byte[] keyArray;

                byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(texto);

                //Se utilizan las clases de encriptación MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(llaveEncriptar));

                hashmd5.Clear();

                //Algoritmo TripleDES
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();

                byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);

                tdes.Clear();

                //se regresa el resultado en forma de una cadena
                texto = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
            } catch (Exception ex) {
                return "Ocurrió un error: " + ex.Message;
            }

            return texto;
        }


        public static string Desencriptar(string llaveDesencriptar, string textoEncriptado) {
            try {
                //string key = "banorteonfnoi23nro2389912837!";
                byte[] keyArray;
                byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

                //algoritmo MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(llaveDesencriptar));

                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

                tdes.Clear();
                textoEncriptado = UTF8Encoding.UTF8.GetString(resultArray);
            } catch (Exception ex) {
                return "Ocurrió un error: " + ex.Message;
            }

            return textoEncriptado;
        }

        public static string GeneraLlave(int idUsuario) {
            string llave = "AXX1B4N0RT3";

            llave += idUsuario.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + 
                DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();

            return llave;
        }

        public static dynamic EncriptaResponse(ActiveResponse obj, string llave) {
            PropertyInfo[] properties = typeof(ActiveResponse).GetProperties();
            Type objReflect = obj.GetType();

            foreach (PropertyInfo p in properties) {
                string aux = p.PropertyType.Name;

                if(p.PropertyType.Name == "String") {
                    try
                    {
                        objReflect.GetProperty(p.Name).SetValue(obj, Encriptar(llave, p.GetValue(obj).ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(aux);
                    }
                }
            }

            return obj;
        }
    }
}
