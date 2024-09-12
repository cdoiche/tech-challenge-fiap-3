using Fiap.Api.Entities;
using Fiap.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Fiap.Api.Interfaces
{
    public interface IContatoRepository
    {
        Task<bool> InserirContato(Contato contato);
        Task<Contato> AtualizarContato(AtualizarContatoSchema contato);
        Task<bool> ExcluirContato(int id);
        IEnumerable<Contato> ConsultarContatosPorDDD(string ddd);
        bool ContatoExiste(string nome, string ddd, string telefone, string email);
        Contato CriarContato(string nome, string ddd, string telefone, string email);
        bool ContatoValido(Contato contato);
        List<ValidationResult> ValidarContato(Contato contato);
    }
}
