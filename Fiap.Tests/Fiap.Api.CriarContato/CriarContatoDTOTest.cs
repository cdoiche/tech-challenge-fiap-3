using Fiap.Api.AlterarContato.DTO;

namespace Fiap.Tests.Fiap.Api.CriarContato
{
    public class CriarContatoDTOTests
    {
        [Fact]
        public void SetId_WithEmptyString_ThrowsException()
        {
            // Arrange
            var dto = new AlterarContatoDTO();

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => dto.Id = null);
            Assert.Equal("Id inválido", exception.Message);
        }

        [Fact]
        public void SetId_WithValidValue_DoesNotThrowException()
        {
            // Arrange
            var dto = new AlterarContatoDTO();

            // Act
            dto.Id = 123;

            // Assert
            Assert.Equal(123, dto.Id);
        }

        [Fact]
        public void SetEmail_WithEmptyString_ThrowsException()
        {
            // Arrange
            var dto = new AlterarContatoDTO();

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => dto.Email = "");
            Assert.Equal("Email inválido", exception.Message);
        }

        [Fact]
        public void SetEmail_WithoutAtSymbol_ThrowsException()
        {
            // Arrange
            var dto = new AlterarContatoDTO();

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => dto.Email = "invalidemail.com");
            Assert.Equal("Email inválido", exception.Message);
        }

        [Fact]
        public void SetEmail_WithValidEmail_DoesNotThrowException()
        {
            // Arrange
            var dto = new AlterarContatoDTO();

            // Act
            dto.Email = "test@example.com";

            // Assert
            Assert.Equal("test@example.com", dto.Email);
        }
    }
}
