using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Data.Repositorio
{
    public class UsuarioRefreshTokenRepositorio
    {
        private readonly Context _context;
        public UsuarioRefreshTokenRepositorio(Context context)
        {
            _context = context;
        }
        public async Task<UsuarioRefreshToken?> Obter(int usuarioId)
        {
            try
            {
                return await _context.UsuarioRefreshToken.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public async Task<UsuarioRefreshToken?> Adicionar(UsuarioRefreshToken usuarioRefreshToken)
        {
            try
            {
               await _context.UsuarioRefreshToken.AddAsync(usuarioRefreshToken);
               await _context.SaveChangesAsync();

               return usuarioRefreshToken;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public async Task<UsuarioRefreshToken?> Atualizar(UsuarioRefreshToken usuarioRefreshToken)
        {
            try
            {
                 _context.Update(usuarioRefreshToken);
                await _context.SaveChangesAsync();
                return usuarioRefreshToken;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
