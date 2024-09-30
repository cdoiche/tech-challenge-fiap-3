using System.Diagnostics;

namespace Fiap.Api.Atualizarcontato.Configuration
{
    public class DiagnosticsConfig
    {
        public const string ServiceName = "AtualizarContatoFiapApiService";
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
