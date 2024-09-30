using Fiap.Core.DTO;
using Fiap.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Fiap.Core.Interfaces
{
    public interface IContatoRepository
    {
        Task<bool> InserirContato(Contato contato);
        Task<Contato> AtualizarContato(AlterarContatoDTO contato);
        Task<bool> ExcluirContato(int id);
        IEnumerable<Contato> ConsultarContatosPorDDD(string ddd);
        Task<bool> ContatoExistePorEmail(string email, int id);
        Task<bool> ContatoExistePorTelefone(string ddd, string telefone);
        bool ContatoExiste(string nome, string ddd, string telefone, string email);
        Contato CriarContato(string nome, string ddd, string telefone, string email);
        bool ContatoValido(Contato contato);
        List<ValidationResult> ValidarContato(Contato contato);
    }
}
