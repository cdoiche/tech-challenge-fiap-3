namespace Fiap.Api.AlterarContato.DTO
{
    public class AlterarContatoDTO
    {
        public int? Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Ddd { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}