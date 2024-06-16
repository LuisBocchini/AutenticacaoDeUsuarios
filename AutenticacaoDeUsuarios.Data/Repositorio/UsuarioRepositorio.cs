using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDeUsuarios.Data.Repositorio
{
    public class UsuarioRepositorio
    {
        private readonly Context _context;
        public UsuarioRepositorio(Context context)
        {
            _context = context;
        }
        public async Task<Usuario?> Adicionar(Usuario usuario)
        {
            try
            {
                await _context.AddAsync(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public async Task<Usuario?> Obter(string? email)
        {
            try
            {
                return await _context.Usuario.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Usuario?> Atualizar(Usuario usuario)
        {
            try
            {
                _context?.Update(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
