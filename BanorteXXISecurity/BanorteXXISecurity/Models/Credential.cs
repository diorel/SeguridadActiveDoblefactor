using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BanorteXXISecurity.Models
{
    public class Credential
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string App { get; set; }
        public int ExpiracionToken { get; set; }

    }
}
