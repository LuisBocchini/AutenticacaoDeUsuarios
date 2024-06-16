using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Data.Models
{
    public class UsuarioRefreshToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? RefreshToken { get; set; }
    }
}
