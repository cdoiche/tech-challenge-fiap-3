using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiap.Api.Entities
{
    [Table("Contato")]
    public class Contato
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Definindo Id como autoincremento
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string Nome { get; set; }

        [RegularExpression(@"[1-9]{2}$", ErrorMessage = "O DDD deve possuir 2 caracteres numéricos")]
        public string Ddd { get; set; }

        [RegularExpression(@"[9]{0,1}[0-9]{4}\-[0-9]{4}$", ErrorMessage = "O telefone deve estar no formato XXXXX-XXXX (celular) ou XXXX-XXXX (fixo)")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "O email não está em um formato válido")]
        public string Email { get; set; }
    }
}