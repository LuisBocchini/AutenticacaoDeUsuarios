using AutenticacaoDeUsuarios.Models;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi.Retorno;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace AutenticacaoDeUsuarios.MetodosDeExtensao
{
    public static class MetodosExtensao
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static async Task<ConfigTokenUsuario?> TokenUsuario(this UsuarioAplicacao str, UsuarioAplicacao? usuario)
        {
            try
            {
                if (usuario == null)
                    return null;

                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    tokenExpirado = usuario?.Token,
                    refreshToken = usuario?.RefreshToken,
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PostAsync($"https://localhost:7053/api/usuario/refreshToken", conteudoChamada);

                var retornoToken = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiToken>();

                if (retornoToken.Sucesso)
                {
                    return new ConfigTokenUsuario()
                    {
                        Token = retornoToken?.Dados?.Token,
                        RefreshToken = retornoToken?.Dados?.RefreshToken,
                        DataExpiracaoToken = retornoToken?.Dados?.DataExpiracao
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
