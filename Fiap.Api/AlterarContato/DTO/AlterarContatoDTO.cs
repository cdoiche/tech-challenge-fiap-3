using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiap.Api.AlterarContato.DTO
{
    public class AlterarContatoDTO
    {
        public string Id { get; set; }
        public string? Nome { get; set; }
        public string? Ddd { get; set; }
        public string? Telefone { get; set; }
        public string Email { get; set; }
    }
}