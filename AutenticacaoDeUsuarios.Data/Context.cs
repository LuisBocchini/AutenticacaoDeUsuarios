using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Data
{
        public class Context : DbContext
        {
            public Context(DbContextOptions<Context> options) : base(options) { }
            public DbSet<Usuario> Usuario { get; set; } = null!;
            public DbSet<UsuarioRefreshToken> UsuarioRefreshToken { get; set; } = null!;
        }
    
}
