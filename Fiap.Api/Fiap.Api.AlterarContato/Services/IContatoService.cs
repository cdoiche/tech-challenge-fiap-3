namespace Fiap.Api.AlterarContato.Services
{
    public interface IContatoService
    {
        Task<HttpResponseMessage> ValidarContatoAsync(int id, string ddd, string telefone, string email);
        Task<HttpResponseMessage> ValidarContatoIdAsync(int id);
    }
}
