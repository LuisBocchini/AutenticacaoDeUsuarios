using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using AutenticacaoDeUsuarios.Models;
using AutenticacaoDeUsuarios.Business.Mensagens;
using AutenticacaoDeUsuarios.Servicos.UsuarioApi.Retorno;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AutenticacaoDeUsuarios.Servicos.UsuarioApi
{
    public class UsuarioApiServico
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _config;
        private string UrlApi;
        private string UrlApiSenha;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public UsuarioApiServico(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration;
            UrlApi = $"{_config["UrlBaseApi"]}/api/usuario";
            UrlApiSenha = $"{_config["UrlBaseApi"]}/api/senha";
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(UsuarioAplicacao? usuario, string? mensagem)> Autenticar(string email, string senha)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    email = email,
                    senha = senha,
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PostAsync($"{UrlApi}/autenticar", conteudoChamada);

                var retornoAutenticacao = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiAutenticacao>();

                if (retornoAutenticacao != null && retornoAutenticacao.Sucesso)
                {
                    var usuario = new UsuarioAplicacao
                    {
                        Id = retornoAutenticacao?.Dados?.Usuario?.Id,
                        Email = retornoAutenticacao?.Dados?.Usuario?.Email,
                        Nome = retornoAutenticacao?.Dados?.Usuario?.Nome,
                        Imagem = retornoAutenticacao?.Dados?.Usuario?.Imagem,
                        DataCadastro = retornoAutenticacao?.Dados?.Usuario?.DataCadastro,
                        Token = retornoAutenticacao?.Dados?.TokenUsuario?.Token,
                        RefreshToken = retornoAutenticacao?.Dados?.TokenUsuario?.RefreshToken,
                        DataExpiracaoToken = retornoAutenticacao?.Dados?.TokenUsuario?.DataExpiracao
                    };
                    return (usuario, retornoAutenticacao?.Mensagem);
                }

                return (null, retornoAutenticacao?.Mensagem);
            }
            catch (Exception)
            {
                return (null, Info.ErroAutenticacao);
                throw;
            }

        }
        public async Task<(UsuarioAplicacao? usuario, string? mensagem)> Cadastrar(string? nome, string? email, string? senha)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    nome = nome,
                    email = email,
                    senha = senha,
                    urlRedirecionamento = "https://localhost:7066/"
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PostAsync($"{UrlApi}/cadastrar", conteudoChamada);

                var retornoCadastro = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoCadastro != null && retornoCadastro.Sucesso)
                {
                    var usuario = new UsuarioAplicacao
                    {
                        Id = retornoCadastro?.Dados?.Usuario?.Id,
                        Email = retornoCadastro?.Dados?.Usuario?.Email,
                        Nome = retornoCadastro?.Dados?.Usuario?.Nome,
                        Imagem = retornoCadastro?.Dados?.Usuario?.Imagem,
                        DataCadastro = retornoCadastro?.Dados?.Usuario?.DataCadastro,
                    };
                    return (usuario, retornoCadastro?.Mensagem);
                }
                return (null, retornoCadastro?.Mensagem);
            }
            catch (Exception)
            {
                return (null, Info.ErroCadastro);
                throw;
            }

        }

        public async Task<(bool sucesso, string? mensagem)> EnviarEmailRedefinicaoSenha(string? email)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    email = email,
                    urlRedirecionamento = "https://localhost:7066/AlterarSenha"
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PostAsync($"{UrlApiSenha}/enviarEmailRedefinicaoSenha", conteudoChamada);

                var retornoCadastro = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoCadastro != null && retornoCadastro.Sucesso)
                {
                    return (true, retornoCadastro?.Mensagem);
                }
                return (false, retornoCadastro?.Mensagem);
            }
            catch (Exception)
            {
                return (false, Info.ErroInterno);
                throw;
            }

        }
        public async Task<(UsuarioAplicacao? usuario, string? mensagem)> Consultar(string? email)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterToken());
                using var respostaChamada = await _httpClient.GetAsync($"{UrlApi}/consultar?email={email}");

                var retornoConsulta = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoConsulta != null && retornoConsulta.Sucesso)
                {
                    var usuario = new UsuarioAplicacao
                    {
                        Id = retornoConsulta?.Dados?.Usuario?.Id,
                        Email = retornoConsulta?.Dados?.Usuario?.Email,
                        Nome = retornoConsulta?.Dados?.Usuario?.Nome,
                        Imagem = retornoConsulta?.Dados?.Usuario?.Imagem,
                        DataCadastro = retornoConsulta?.Dados?.Usuario?.DataCadastro,
                    };
                    return (usuario, retornoConsulta?.Mensagem);
                }
                return (null, retornoConsulta?.Mensagem);
            }
            catch (Exception)
            {
                return (null, Info.ErroConsulta);
                throw;
            }
        }
        public async Task<(UsuarioAplicacao? usuario, string? mensagem)> Editar(string? email, string? nome, string? imagem)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    nome = nome,
                    email = email,
                    imagem = imagem,
                }),
               Encoding.UTF8,
               "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterToken());
                using var respostaChamada = await _httpClient.PutAsync($"{UrlApi}/editar", conteudoChamada);

                var retornoEdicao = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoEdicao.Sucesso)
                {
                    var usuario = new UsuarioAplicacao
                    {
                        Id = retornoEdicao?.Dados?.Usuario?.Id,
                        Email = retornoEdicao?.Dados?.Usuario?.Email,
                        Nome = retornoEdicao?.Dados?.Usuario?.Nome,
                        Imagem = retornoEdicao?.Dados?.Usuario?.Imagem,
                        DataCadastro = retornoEdicao?.Dados?.Usuario?.DataCadastro,
                    };
                    return (usuario, retornoEdicao.Mensagem);
                }
                return (null, retornoEdicao.Mensagem);
            }
            catch (Exception)
            {
                return (null, Info.ErroConsulta);
                throw;
            }
        }
        public async Task<ConfigTokenUsuario?> ObterOuAtualizarTokens(string? token, string? refreshToken, DateTime dataExpiracao)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                    return null;

                if (dataExpiracao > DateTime.Now)
                    return new ConfigTokenUsuario() { Token = token, RefreshToken = refreshToken, DataExpiracaoToken = dataExpiracao };

                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    tokenExpirado = token,
                    refreshToken = refreshToken,
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PostAsync($"https://localhost:7053/api/usuario/refreshToken", conteudoChamada);

                var retornoToken = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiToken>();

                if (retornoToken != null && retornoToken.Sucesso)
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

        public async Task<bool> AtualizarSenha(string? senhaAtual, string? novaSenha, string? email)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                   email = email,
                   senhaAtual = senhaAtual,
                   novaSenha = novaSenha
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterToken());
                using var respostaChamada = await _httpClient.PutAsync($"{UrlApiSenha}/atualizarSenha", conteudoChamada);

                var retornoCadastro = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoCadastro != null && retornoCadastro.Sucesso)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        public async Task<(bool sucesso, string? mensagem)> RedefinirSenha(string? novaSenha, string? token)
        {
            try
            {
                StringContent conteudoChamada = new(JsonSerializer.Serialize(new
                {
                    token = token,
                    novaSenha = novaSenha
                }),
                Encoding.UTF8,
                "application/json");

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var respostaChamada = await _httpClient.PutAsync($"{UrlApiSenha}/redefinirSenha", conteudoChamada);

                var retornoRedefinicaoSenha = await respostaChamada.Content.ReadFromJsonAsync<RetornoUsuarioApiCadastro>();

                if (retornoRedefinicaoSenha != null && retornoRedefinicaoSenha.Sucesso)
                {
                    return (true, retornoRedefinicaoSenha?.Mensagem);
                }
                return (false, retornoRedefinicaoSenha?.Mensagem);
            }
            catch (Exception)
            {
                return (false, Info.ErroInterno);
                throw;
            }

        }

        public async void AtualizarClaimsUsuario(string? token, string? refreshToken, string? dataExpiracao, ClaimsPrincipal usuario)
        {

            var claimsIdentity = (ClaimsIdentity?)usuario.Identity;
            var claimToken = claimsIdentity?.FindFirst("Token");
            var claimRefreshToken = claimsIdentity?.FindFirst("RefreshToken");
            var claimDataExpiracaoToken = claimsIdentity?.FindFirst("DataExpiracaoToken");

            claimsIdentity?.RemoveClaim(claimToken);
            claimsIdentity?.AddClaim(new Claim("Token", token));

            claimsIdentity?.RemoveClaim(claimRefreshToken);
            claimsIdentity?.AddClaim(new Claim("RefreshToken", refreshToken));

            claimsIdentity?.RemoveClaim(claimDataExpiracaoToken);
            claimsIdentity?.AddClaim(new Claim("DataExpiracaoToken", dataExpiracao));

            await _httpContextAccessor?.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario);
        }
        
        private async Task<string?> ObterToken()
        {
            var usuario = _httpContextAccessor?.HttpContext?.User;

            if (usuario?.Identity?.IsAuthenticated ?? false)
            {
                var claimTokenValue = ((ClaimsIdentity)usuario.Identity).FindFirst("Token")?.Value;
                var claimRefreshTokenValue = ((ClaimsIdentity)usuario.Identity).FindFirst("RefreshToken")?.Value;
                var claimDataExpiracaoTokenValue = ((ClaimsIdentity)usuario.Identity).FindFirst("DataExpiracaoToken")?.Value;
                var configToken = await ObterOuAtualizarTokens(claimTokenValue, claimRefreshTokenValue, Convert.ToDateTime(claimDataExpiracaoTokenValue));
                if(configToken?.Token != null)
                    AtualizarClaimsUsuario(configToken?.Token, configToken?.RefreshToken, configToken?.DataExpiracaoToken.ToString(), usuario);
                return configToken?.Token;
            }
            return null;
        }
    }
}
