using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Data.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public string? Senha { get; set; }
        public string? Imagem { get; set; }
        public DateTime? DataCadastro { get; set; }
        public bool Ativo { get; set; }
    }

}
