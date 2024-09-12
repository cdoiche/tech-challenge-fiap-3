using Fiap.Api.Entities;
using Fiap.Domain.Repositories;
using Fiap.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Threading.Tasks;
using Fiap.Api.Models;

namespace Fiap.Tests
{
    public class ContatoRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<FiapDataContext> _options;
        private FiapDataContext _context;

        public ContatoRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<FiapDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new FiapDataContext(_options);
        }

        private void SeedTestData()
        {
            _context.Contatos.Add(new Contato { Id = 1, Nome = "Ana", Ddd = "11", Telefone = "123456789", Email = "ana@email.com" });
            _context.Contatos.Add(new Contato { Id = 2, Nome = "Bruno", Ddd = "21", Telefone = "987654321", Email = "bruno@email.com" });
            _context.Contatos.Add(new Contato { Id = 3, Nome = "Carlos", Ddd = "11", Telefone = "111111111", Email = "carlos@email.com" });
            _context.SaveChanges();
        }

        [Fact]
        public async Task InserirContato_ShouldAddContato_WhenDataIsValid()
        {
            // Arrange
            SeedTestData();
            var contato = new Contato { Nome = "Novo Contato", Ddd = "31", Telefone = "998877665", Email = "novoteste@email.com" };
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.InserirContato(contato);

            // Assert
            Assert.True(result);
            Assert.Equal(4, _context.Contatos.Count());
        }

        [Fact]
        public async Task InserirContato_ShouldReturnFalse_WhenDataIsInvalid()
        {
            // Arrange (empty contact)
            SeedTestData();
            var contato = new Contato();
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.InserirContato(contato);

            // Assert
            Assert.False(result);
            Assert.Equal(3, _context.Contatos.Count());
        }

        [Fact]
        public async Task AtualizarContato_ShouldUpdateContato_WhenDataIsValid()
        {
            // Arrange
            SeedTestData();
            var contato = new AtualizarContatoSchema
            {
                Id = 1,
                Nome = "Updated Nome",
                Ddd = "12",
                Telefone = "111111111",
                Email = "updated1@email.com"
            };
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.AtualizarContato(contato);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Nome", result.Nome);
            Assert.Equal("12", result.Ddd);
            Assert.Equal("111111111", result.Telefone);
            Assert.Equal("updated1@email.com", result.Email);
        }

        [Fact]
        public async Task AtualizarContato_ShouldReturnNull_WhenContatoDoesNotExist()
        {
            // Arrange
            SeedTestData();
            var contato = new AtualizarContatoSchema
            {
                Id = 999, // Non-existent contact
                Nome = "Non Existent",
                Ddd = "00",
                Telefone = "000000000",
                Email = "nonexistent@email.com"
            };
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.AtualizarContato(contato);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ExcluirContato_ShouldReturnTrue_WhenContatoExists()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.ExcluirContato(1);

            // Assert
            Assert.True(result);
            Assert.Equal(2, _context.Contatos.Count()); // One contact should be deleted
            Assert.Null(_context.Contatos.FirstOrDefault(x => x.Id == 1)); // Contact with Id 1 should not exist
        }

        [Fact]
        public async Task ExcluirContato_ShouldReturnFalse_WhenContatoDoesNotExist()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);

            // Act
            var result = await repository.ExcluirContato(999); // Non-existent contact ID

            // Assert
            Assert.False(result);
            Assert.Equal(3, _context.Contatos.Count()); // No contact should be deleted
        }

        [Fact]
        public void ConsultarContatosPorDDD_ShouldReturnFilteredAndOrderedContatos_WhenDDDIsProvided()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);

            // Act
            var result = repository.ConsultarContatosPorDDD("11");

            // Assert
            Assert.Equal(2, result.Count()); // There should be 2 contacts with DDD "11"
            Assert.Equal("Ana", result.First().Nome); // The first contact should be Ana
            Assert.Equal("Carlos", result.Last().Nome); // The second contact should be Carlos
        }

        [Fact]
        public void ConsultarContatosPorDDD_ShouldReturnAllOrderedContatos_WhenDDDIsNotProvided()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);

            // Act
            var result = repository.ConsultarContatosPorDDD(string.Empty);

            // Assert
            Assert.Equal(3, result.Count()); // There should be 3 contacts in total
            Assert.Equal("Ana", result.First().Nome); // The first contact should be Ana
            Assert.Equal("Carlos", result.Last().Nome); // The last contact should be Carlos
        }

        [Fact]
        public void CriarContato_ShouldReturnNewContato_WhenContatoDoesNotExist()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);
            var nome = "Daniel";
            var ddd = "31";
            var telefone = "222333444";
            var email = "daniel@email.com";

            // Act
            var result = repository.CriarContato(nome, ddd, telefone, email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nome, result.Nome);
            Assert.Equal(ddd, result.Ddd);
            Assert.Equal(telefone, result.Telefone);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public void CriarContato_ShouldReturnNull_WhenContatoAlreadyExists()
        {
            // Arrange
            SeedTestData(); // Seed data for the test
            var repository = new ContatoRepository(_context);
            var nome = "Ana";
            var ddd = "11";
            var telefone = "123456789";
            var email = "ana@email.com";

            // Assuming ContatoExiste checks for existing contacts
            bool ContatoExiste() => _context.Contatos.Any(c => c.Nome == nome && c.Ddd == ddd && c.Telefone == telefone && c.Email == email);

            // Act
            var result = repository.CriarContato(nome, ddd, telefone, email);

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
