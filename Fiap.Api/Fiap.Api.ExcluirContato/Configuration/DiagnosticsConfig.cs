using System.Diagnostics;

namespace Fiap.Api.ExcluirContato.Configuration
{
    public class DiagnosticsConfig
    {
        public const string ServiceName = "FiapApiService";
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
