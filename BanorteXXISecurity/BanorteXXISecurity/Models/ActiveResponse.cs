using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BanorteXXISecurity.Models
{
    public class ActiveResponse
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Descripcion { get; set; }
        public string Oficina { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string NumNomina { get; set; }
        public string Compañia { get; set; }
        public string Departamento { get; set; }
        public string Puesto { get; set; }
        public string Notas { get; set; }
        public List<Application> Aplicaciones { get; set; }
        public Permission Permisos { get; set; }
        public List<InformationToken> InformacionToken { get; set; }
        public string Foto { get; set; }
        public ResponseQRGoogle CodigoQR { get; set; }
        public int IDUsuario { get; set; }
        public string Llave { get; set; }

    }
}
