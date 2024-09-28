using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Fiap.Core.Interfaces;
using Fiap.Core.Context;
using Fiap.Core.Entities;
using Fiap.Core.DTO;

namespace Fiap.Domain.Repositories
{
    public class ContatoRepository : IContatoRepository
    {
        private readonly FiapDataContext _context;

        public ContatoRepository(FiapDataContext context)
        {
            _context = context;
        }

        public async Task<bool> InserirContato(Contato contato)
        {
            // Verificar se os campos obrigatórios estão preenchidos
            if (string.IsNullOrEmpty(contato.Nome) ||
                string.IsNullOrEmpty(contato.Ddd) ||
                string.IsNullOrEmpty(contato.Telefone) ||
                string.IsNullOrEmpty(contato.Email))
            {
                return false;
            }

            // Verificar se o e-mail já está em uso por outro contato
            if (await ContatoExistePorEmail(contato.Email, 0))
            {
                throw new InvalidOperationException("O e-mail já está sendo usado por outro contato.");
            }

            // Verificar se o telefone já está em uso por outro contato
            if (await ContatoExistePorTelefone(contato.Ddd, contato.Telefone))
            {
                throw new InvalidOperationException("O telefone já está sendo usado por outro contato.");
            }

            // Adicionar o contato ao contexto e salvar as mudanças
            await _context.Contatos.AddAsync(contato);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Contato> AtualizarContato(AtualizarContatoDTO contato)
        {
            // Obter o contato existente pelo ID
            var contatoExistente = await _context.Contatos.FindAsync(contato.Id);

            if (contatoExistente != null)
            {
                // Atualizar os campos do contato com os novos valores, se forem fornecidos
                if (!string.IsNullOrEmpty(contato.Nome))
                    contatoExistente.Nome = contato.Nome;

                if (!string.IsNullOrEmpty(contato.Ddd))
                    contatoExistente.Ddd = contato.Ddd;

                if (!string.IsNullOrEmpty(contato.Telefone))
                    contatoExistente.Telefone = contato.Telefone;

                if (!string.IsNullOrEmpty(contato.Email))
                {
                    // Verificar se o novo e-mail já está em uso por outro contato
                    if (await ContatoExistePorEmail(contato.Email, contato.Id))
                    {
                        throw new InvalidOperationException("O e-mail já está sendo usado por outro contato.");
                    }

                    contatoExistente.Email = contato.Email;
                }

                if (ContatoValido(contatoExistente))
                {
                    // Salvar as mudanças no banco de dados
                    await _context.SaveChangesAsync();

                    return contatoExistente;
                }
                else
                {
                    string validacao = String.Join(System.Environment.NewLine, ValidarContato(contatoExistente));
                    throw new InvalidOperationException(validacao);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ExcluirContato(int id)
        {
            // Obter o contato pelo ID
            var contatoExistente = await _context.Contatos.FindAsync(id);

            if (contatoExistente != null)
            {
                // Remover o contato do contexto e salvar as mudanças
                _context.Contatos.Remove(contatoExistente);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Contato> ConsultarContatosPorDDD(string ddd)
        {
            // Filtrar os contatos pelo DDD e ordenar pelo nome
            if (!string.IsNullOrEmpty(ddd))
            {
                Console.WriteLine("Fazendo Where na tabela Contatos");
                return _context.Contatos.Where(x => x.Ddd == ddd).OrderBy(x => x.Nome);
            }
            else
            {
                return _context.Contatos.OrderBy(x => x.Nome);
            }
        }

        public async Task<bool> ContatoExistePorEmail(string email, int id)
        {
            // Verificar se existe algum contato com o mesmo e-mail no banco de dados
            return await _context.Contatos.AnyAsync(c => c.Email == email && c.Id != id);
        }

        public async Task<bool> ContatoExistePorTelefone(string ddd, string telefone)
        {
            // Verificar se existe algum contato com o mesmo telefone no banco de dados
            return await _context.Contatos.AnyAsync(c => c.Ddd == ddd && c.Telefone == telefone);
        }

        public Contato CriarContato(string nome, string ddd, string telefone, string email)
        {
            // Criar um novo contato se ele não existir
            if (!ContatoExiste(nome, ddd, telefone, email))
            {
                Contato contato = new Contato();
                contato.Nome = nome;
                contato.Ddd = ddd;
                contato.Telefone = telefone;
                contato.Email = email;

                return contato;
            }
            else
            {
                return null;
            }
        }

        public bool ContatoExiste(string nome, string ddd, string telefone, string email)
        {
            // Verificar se existe algum contato com as mesmas informações no banco de dados
            return _context.Contatos.Any(c => c.Nome == nome && c.Ddd == ddd && c.Telefone == telefone && c.Email == email);
        }

        public bool ContatoValido(Contato contato)
        {
            List<ValidationResult> listResultado = ValidarContato(contato);
            return !listResultado.Any();
        }

        public List<ValidationResult> ValidarContato(Contato contato)
        {
            var contexto = new ValidationContext(contato);
            var listResultado = new List<ValidationResult>();

            Validator.TryValidateObject(contato, contexto, listResultado, true);

            return listResultado;
        }
    }
}
