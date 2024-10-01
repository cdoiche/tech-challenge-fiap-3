namespace Fiap.Core.DTO
{
    public class AlterarContatoDTO
    {
        public string Id { get; } = string.Empty;
        public string? Nome { get; set; }
        public string? Ddd { get; set; }
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}