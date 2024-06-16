using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace AutenticacaoDeUsuarios.Business.Servicos
{
    public class EmailServico
    {
        private readonly IConfiguration Configuration;
        public EmailServico(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public (bool sucesso, string? mensagem) EnviarEmail(string? assunto, string? corpoEmail, string? destinatario)
        {
            var smtpHost = Configuration["Smtp:Host"];
            var smtpPort = int.Parse(Configuration["Smtp:Porta"]);
            var smtpUsername = Configuration["Smtp:Username"];
            var smtpPassword = Configuration["Smtp:Password"];
            var NomeConta = Configuration["Smtp:NomeConta"];
            var credenciais = new NetworkCredential(smtpUsername, smtpPassword);
        
            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                var remetente = new MailAddress(smtpUsername, NomeConta);
                var emailDestinatario = new MailAddress(destinatario);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credenciais;
                smtpClient.EnableSsl = true;

                MailMessage mensagem = new MailMessage(remetente, emailDestinatario);
                mensagem.IsBodyHtml = true;
                mensagem.Subject = assunto;
                mensagem.Body = corpoEmail;

                try
                {
                    smtpClient.Send(mensagem);
                    return (true, null);
                }
                catch (Exception)
                {
                    return (false, "Erro ao enviar email");
                }
            }
        }
       
    }
}
