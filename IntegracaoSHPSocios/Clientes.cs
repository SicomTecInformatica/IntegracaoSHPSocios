using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoSHPSocios
{
    public class Clientes
    {
        public string id { get; set; }
        public string name { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string titulo { get; set; }
        public string data { get; set; }
        public string nascimento { get; set; }
        public string telefone { get; set; }
        public string socio { get; set; }
        public Imagem imagem { get; set; }
     }
    public class Imagem
    {
        public string type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string path { get; set; }
        public string url { get; set; }
        public string folder { get; set; }
        public string fileType { get; set; }
        public string isText { get; set; }
        public string size { get; set; }
        public string encoding { get; set; }
        public string isInactive { get; set; }
        public string isOnline { get; set; }
    }
    public class RootObject
    {
        public List<Clientes> Clientes { get; set; }
        //public List<Imagem> Imagem { get; set; }
    }
}
