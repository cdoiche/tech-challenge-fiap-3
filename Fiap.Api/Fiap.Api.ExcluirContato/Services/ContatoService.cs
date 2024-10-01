using System.Net.Http;
using System.Threading.Tasks;

namespace Fiap.Api.ExcluirContato.Services { 
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

        public async Task<HttpResponseMessage> ValidarContatoIdAsync(int id)
        {
            var requestUri = $"{_consultarServiceUrl}/ValidarContatoId?id={id}";
            return await _httpClient.GetAsync(requestUri);
        }
    }
}