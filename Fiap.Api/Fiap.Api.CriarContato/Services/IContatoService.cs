namespace Fiap.Api.CriarContato.Services
{
    public interface IContatoService
    {
        Task<HttpResponseMessage> ValidarContatoAsync(string ddd, string telefone, string email);
    }
}
