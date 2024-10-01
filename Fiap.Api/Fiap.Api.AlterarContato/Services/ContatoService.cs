using System.Net.Http;
using System.Threading.Tasks;

namespace Fiap.Api.AlterarContato.Services { 
    public class ContatoService : IContatoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _consultarServiceUrl;

        public ContatoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _consultarServiceUrl = Environment.GetEnvironmentVariable("ConsultarServiceUrl")
                                ?? throw new InvalidOperationException("Environment variable 'ConsultarServiceUrl' is not set.");
        }

        public async Task<HttpResponseMessage> ValidarContatoAsync(int id, string ddd, string telefone, string email)
        {
            var requestUri = $"{_consultarServiceUrl}/ValidarContato?id={id}&ddd={ddd}&telefone={telefone}&email={email}";
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> ValidarContatoIdAsync(int id)
        {
            var requestUri = $"{_consultarServiceUrl}/ValidarContatoId?id={id}";
            return await _httpClient.GetAsync(requestUri);
        }
    }
}