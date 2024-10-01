using Fiap.Api.CriarContato.Configuration;
using Fiap.Api.CriarContato.Controllers;
using Fiap.Api.CriarContato.DTO;
using Fiap.Api.CriarContato.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.Tests.Fiap.Api.CriarContato
{
    public class CriarContatoControllerTest
    {
        private readonly Mock<Instrumentor> _mockInstrumentor;
        private readonly Mock<IContatoService> _mockContatoService;
        private readonly Mock<IConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IModel> _mockChannel;
        private readonly Mock<IConnection> _mockConnection;

        public CriarContatoControllerTest()
        {
            _mockInstrumentor = new Mock<Instrumentor>();
            _mockContatoService = new Mock<IContatoService>();
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();
        }

        [Fact]
        public async Task CriarContato_MissingFields_ReturnsBadRequest()
        {
            // Arrange
            var controller = new CriarContatoController(_mockInstrumentor.Object, _mockContatoService.Object);

            var dto = new CriarContatoDTO
            {
                Nome = "",
                Ddd = "11",
                Telefone = "999999999",
                Email = "test@example.com"
            };

            // Act
            var result = await controller.CriarContatoAsync(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Todos os campos são de preenchimento obrigatório.", badRequestResult.Value);
        }

        [Fact]
        public async Task CriarContato_ValidatesContactAndPublishesMessage_ReturnsOk()
        {
            // Arrange
            var dto = new CriarContatoDTO
            {
                Nome = "Teste",
                Ddd = "11",
                Telefone = "999999999",
                Email = "test@example.com"
            };

            // Setup mock to return a successful response
            _mockContatoService
                .Setup(service => service.ValidarContatoAsync(dto.Ddd, dto.Telefone, dto.Email))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)); // Simulating success

            // Setup mock connection and channel for RabbitMQ
            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            var controller = new CriarContatoController(_mockInstrumentor.Object, _mockContatoService.Object)
            {
                // Injecting mock connection factory
                ConnectionFactory = _mockConnectionFactory.Object
            };

            // Act
            var result = await controller.CriarContatoAsync(dto);

            // Assert
            Assert.IsType<OkResult>(result);

            // Verify the RabbitMQ publish call
            _mockChannel.Verify(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public async Task CriarContato_InvalidContact_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CriarContatoDTO
            {
                Nome = "Teste",
                Ddd = "11",
                Telefone = "999999999",
                Email = "test@example.com"
            };

            // Setup mock to return a BadRequest response
            _mockContatoService
                .Setup(service => service.ValidarContatoAsync(dto.Ddd, dto.Telefone, dto.Email))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)); // Simulating failure

            var controller = new CriarContatoController(_mockInstrumentor.Object, _mockContatoService.Object);

            // Act
            var result = await controller.CriarContatoAsync(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O e-mail ou telefone informado já existe.", badRequestResult.Value);
        }

        [Fact]
        public async Task CriarContato_InternalServerError_Returns500()
        {
            // Arrange
            var dto = new CriarContatoDTO
            {
                Nome = "Teste",
                Ddd = "11",
                Telefone = "999999999",
                Email = "test@example.com"
            };

            // Setup mock to return a successful response
            _mockContatoService
                .Setup(service => service.ValidarContatoAsync(dto.Ddd, dto.Telefone, dto.Email))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)); // Simulating success

            // Simulate exception when publishing to RabbitMQ
            _mockConnection.Setup(c => c.CreateModel()).Throws(new Exception("RabbitMQ error"));

            var controller = new CriarContatoController(_mockInstrumentor.Object, _mockContatoService.Object)
            {
                // Injecting mock connection factory
                ConnectionFactory = _mockConnectionFactory.Object
            };

            // Act
            var result = await controller.CriarContatoAsync(dto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Falha interna no servidor.", statusCodeResult.Value.ToString());
        }
    }
}
