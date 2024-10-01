namespace Fiap.Api.AlterarContato.Services
{
    public interface IContatoService
    {
        Task<HttpResponseMessage> ValidarContatoAsync(string ddd, string telefone, string email);
    }
}
