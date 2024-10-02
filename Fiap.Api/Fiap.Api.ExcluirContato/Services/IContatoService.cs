namespace Fiap.Api.ExcluirContato.Services
{
    public interface IContatoService
    {
        Task<HttpResponseMessage> ValidarContatoIdAsync(int id);
    }
}
