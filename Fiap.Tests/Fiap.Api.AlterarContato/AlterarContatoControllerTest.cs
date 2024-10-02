using Fiap.Api.AlterarContato.Controllers;
using Fiap.Api.AlterarContato.DTO;
using Fiap.Api.AlterarContato.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using RabbitMQ.Client;
using Fiap.Api.AlterarContato.Configuration;

namespace Fiap.Tests.Fiap.Api.AlterarContato
{
    public class AlterarContatoControllerTests
    {
        private readonly Mock<Instrumentor> _mockInstrumentor;
        private readonly Mock<IModel> _mockChannel;
        private readonly Mock<IContatoService> _mockContatoService;

        public AlterarContatoControllerTests()
        {
            _mockInstrumentor = new Mock<Instrumentor>();
            _mockChannel = new Mock<IModel>();
            _mockContatoService = new Mock<IContatoService>();
        }

        [Fact]
        public void AlterarContato_MissingFields_ReturnsBadRequest()
        {
            // Arrange
            var controller = new AlterarContatoController(_mockInstrumentor.Object, _mockContatoService.Object, _mockChannel.Object);

            var dto = new AlterarContatoDTO
            {
                Id = null,
                Email = ""
            };

            // Act
            var result = controller.AlterarContatoAsync(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os campos Id e E-mail são de preenchimento obrigatório.", badRequestResult.Value);
        }

        [Fact]
        public async void AlterarContato_ValidatesContactAndPublishesMessage_ReturnsOk()
        {
            // Arrange
            var dto = new AlterarContatoDTO
            {
                Id = 123,
                Nome = "Teste",
                Email = "test@example.com",
                Ddd = "11",
                Telefone = "999999999"
            };

            _mockContatoService
                .Setup(service => service.ValidarContatoAsync(dto.Id.GetValueOrDefault(), dto.Ddd, dto.Telefone, dto.Email))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var controller = new AlterarContatoController(_mockInstrumentor.Object, _mockContatoService.Object, _mockChannel.Object);

            // Act
            var result = await controller.AlterarContatoAsync(dto);

            // Assert
            Assert.IsType<OkResult>(result);

            _mockChannel.Verify(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public async void AlterarContato_InvalidContact_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AlterarContatoDTO
            {
                Email = "test@example.com",
                Ddd = "11",
                Telefone = "999999999"
            };

            _mockContatoService
                .Setup(service => service.ValidarContatoAsync(dto.Id.GetValueOrDefault(), dto.Ddd, dto.Telefone, dto.Email))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var controller = new AlterarContatoController(_mockInstrumentor.Object, _mockContatoService.Object, _mockChannel.Object);

            // Act
            var result = await controller.AlterarContatoAsync(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O e-mail ou telefone informado já está sendo usado por outro contato.", badRequestResult.Value);
        }
    }
}
