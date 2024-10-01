using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fiap.Tests
{
    public class ContatoControllerTest
    {
        private readonly Mock<IContatoRepository> _mockContatoRepository;

        public ContatoControllerTest()
        {
            _mockContatoRepository = new Mock<IContatoRepository>();
        }

        [Fact]
        public async Task CriarContato_ShouldReturnOk_WhenContatoIsCreatedSuccessfully()
        {
            // Arrange
            var novoContatoSchema = new CriarContatoSchema { Nome = "Teste", Ddd = "11", Telefone = "99999999", Email = "teste@email.com" };
            _mockContatoRepository.Setup(repo => repo.CriarContato(novoContatoSchema.Nome, novoContatoSchema.Ddd, novoContatoSchema.Telefone, novoContatoSchema.Email))
                .Returns(new Contato { Id = 1, Nome = novoContatoSchema.Nome, Ddd = novoContatoSchema.Ddd, Telefone = novoContatoSchema.Telefone, Email = novoContatoSchema.Email });

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.CriarContato(novoContatoSchema);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var createdContato = (result as OkObjectResult).Value as Contato;
            Assert.NotNull(createdContato);
            Assert.Equal(novoContatoSchema.Nome, createdContato.Nome);
            Assert.Equal(novoContatoSchema.Ddd, createdContato.Ddd);
            Assert.Equal(novoContatoSchema.Telefone, createdContato.Telefone);
            Assert.Equal(novoContatoSchema.Email, createdContato.Email);
        }

        [Fact]
        public async Task CriarContato_ShouldReturnBadRequest_WhenContatoCreationFails()
        {
            // Arrange
            var novoContatoSchema = new CriarContatoSchema { Nome = "Teste", Ddd = "11", Telefone = "99999999", Email = "teste@email.com" };
            _mockContatoRepository.Setup(repo => repo.CriarContato(novoContatoSchema.Nome, novoContatoSchema.Ddd, novoContatoSchema.Telefone, novoContatoSchema.Email))
                .Returns((Contato)null);

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.CriarContato(novoContatoSchema);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CriarContato_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var novoContatoSchema = new CriarContatoSchema { Nome = "Teste", Ddd = "11", Telefone = "99999999", Email = "teste@email.com" };
            _mockContatoRepository.Setup(repo => repo.CriarContato(novoContatoSchema.Nome, novoContatoSchema.Ddd, novoContatoSchema.Telefone, novoContatoSchema.Email))
                .Throws(new Exception("Fake exception"));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.CriarContato(novoContatoSchema);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Falha interna no servidor", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task AtualizarContato_ShouldReturnOk_WhenContatoIsUpdatedSuccessfully()
        {
            // Arrange
            var atualizarContatoSchema = new AtualizarContatoSchema { Id = 1, Nome = "Teste Atualizado", Ddd = "12", Telefone = "88888888", Email = "atualizado@email.com" };
            Contato updatedContato = new Contato { Id = atualizarContatoSchema.Id, Nome = atualizarContatoSchema.Nome, Ddd = atualizarContatoSchema.Ddd, Telefone = atualizarContatoSchema.Telefone, Email = atualizarContatoSchema.Email };
            _mockContatoRepository.Setup(repo => repo.AtualizarContato(atualizarContatoSchema))
                .Returns(Task.FromResult(updatedContato));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.AtualizarContato(atualizarContatoSchema);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var updatedResult = (result as OkObjectResult).Value as Contato;
            Assert.NotNull(updatedResult);
            Assert.Equal(atualizarContatoSchema.Id, updatedResult.Id);
            Assert.Equal(atualizarContatoSchema.Nome, updatedResult.Nome);
            Assert.Equal(atualizarContatoSchema.Ddd, updatedResult.Ddd);
            Assert.Equal(atualizarContatoSchema.Telefone, updatedResult.Telefone);
            Assert.Equal(atualizarContatoSchema.Email, updatedResult.Email);
        }

        [Fact]
        public async Task AtualizarContato_ShouldReturnBadRequest_WhenContatoUpdateFails()
        {
            // Arrange
            var atualizarContatoSchema = new AtualizarContatoSchema { Id = 1, Nome = "Teste Atualizado", Ddd = "12", Telefone = "88888888", Email = "atualizado@email.com" };
            _mockContatoRepository.Setup(repo => repo.AtualizarContato(atualizarContatoSchema))
                .Returns(Task.FromResult<Contato>(null));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.AtualizarContato(atualizarContatoSchema);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AtualizarContato_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var atualizarContatoSchema = new AtualizarContatoSchema { Id = 1, Nome = "Teste Atualizado", Ddd = "12", Telefone = "88888888", Email = "atualizado@email.com" };
            _mockContatoRepository.Setup(repo => repo.AtualizarContato(atualizarContatoSchema))
                .Throws(new Exception("Fake exception"));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.AtualizarContato(atualizarContatoSchema);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Falha interna no servidor", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task ExcluirContato_ShouldReturnOk_WhenContatoIsDeletedSuccessfully()
        {
            // Arrange
            int id = 1;
            _mockContatoRepository.Setup(repo => repo.ExcluirContato(id))
                .Returns(Task.FromResult(true));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.ExcluirContato(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ExcluirContato_ShouldReturnBadRequest_WhenContatoDeletionFails()
        {
            // Arrange
            int id = 1;
            _mockContatoRepository.Setup(repo => repo.ExcluirContato(id))
                .Returns(Task.FromResult(false));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.ExcluirContato(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ExcluirContato_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            int id = 1;
            _mockContatoRepository.Setup(repo => repo.ExcluirContato(id))
                .Throws(new Exception("Fake exception"));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = await controller.ExcluirContato(id);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Falha interna no servidor", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task ConsultarContato_ShouldReturnOk_WithListOfContatos_WhenConsultaSucceeds()
        {
            // Arrange
            var consultarContatoSchema = new ConsultarContatoSchema { Ddd = "11" };
            var expectedList = new List<Contato>() { new Contato { Id = 1, Nome = "Teste", Ddd = "11", Telefone = "123456789", Email = "teste@email.com" } };
            _mockContatoRepository.Setup(repo => repo.ConsultarContatosPorDDD(consultarContatoSchema.Ddd))
                .Returns(expectedList);

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = controller.ConsultarContato(consultarContatoSchema.Ddd);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult.Value);
            Assert.IsType<List<Contato>>(objectResult.Value);
            var contatoList = objectResult.Value as IEnumerable<Contato>;
            Assert.Equal(expectedList.Count(), contatoList.Count()); // Assert same number of items
                                                                     // You can assert individual items in the list if needed, but this demonstrates basic structure
        }

        [Fact]
        public async Task ConsultarContato_ShouldReturnStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var consultarContatoSchema = new ConsultarContatoSchema { Ddd = "11" };
            _mockContatoRepository.Setup(repo => repo.ConsultarContatosPorDDD(consultarContatoSchema.Ddd))
                .Throws(new Exception("Fake exception"));

            var controller = new ContatoController(_mockContatoRepository.Object);

            // Act
            var result = controller.ConsultarContato(consultarContatoSchema.Ddd);

            // Assert
            Assert.IsType<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Falha interna no servidor", statusCodeResult.Value.ToString());
        }

    }
}
