using System.Diagnostics;

namespace Fiap.Api.ConsultarContato.Configuration
{
    public class DiagnosticsConfig
    {
        public const string ServiceName = "FiapApiService";
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
