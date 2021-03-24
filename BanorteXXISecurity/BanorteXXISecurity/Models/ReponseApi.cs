using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BanorteXXISecurity.Models
{
    public class ReponseApi
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public dynamic Respuesta { get; set; }
    }
}
