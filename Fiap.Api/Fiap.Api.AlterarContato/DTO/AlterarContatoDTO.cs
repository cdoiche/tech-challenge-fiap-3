namespace Fiap.Api.AlterarContato.DTO
{
    public class AlterarContatoDTO
    {
        public string Id { 
            get
            {
                return Id;
            }
            set
            {
                if (string.IsNullOrEmpty(Id)) throw new Exception("Id inválido");
            }
        }
        public string? Nome { get; set; }
        public string? Ddd { get; set; }
        public string? Telefone { get; set; }
        public string Email
        {
            get => Email;
            set
            {
                if (string.IsNullOrEmpty(Email)) throw new Exception("Email inválido");
                if (!Email.Contains('@')) throw new Exception("Email inválido");
            }
        }
    }
}