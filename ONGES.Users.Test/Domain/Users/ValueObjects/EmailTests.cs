using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Domain.Users.Exceptions.Email;

namespace ONGES.Users.Test.Domain.Users.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_ValidEmail_ReturnsEmail()
        {
            // Arrange
            var validEmail = "teste@teste.com";

            // Act
            var email = Email.Create(validEmail);

            // Assert
            Assert.Equal(validEmail, email.ToString());
            Assert.NotNull(email);
        }

        [Fact]
        public void Create_InvalidEmail_ThrowsException()
        {
            // Arrange
            var invalidEmail = "invalid-email";

            // Act & Assert
            Assert.Throws<InvalidEmailException>(() => Email.Create(invalidEmail));
        }

        [Fact]
        public void Create_NullOrEmptyEmail_ThrowsException()
        {
            // Arrange
            string nullEmail = null!;
            string emptyEmail = "";
            string whitespaceEmail = "   ";

            // Act & Assert
            Assert.Throws<NullOrEmptyEmailException>(() => Email.Create(nullEmail));
            Assert.Throws<NullOrEmptyEmailException>(() => Email.Create(emptyEmail));
            Assert.Throws<NullOrEmptyEmailException>(() => Email.Create(whitespaceEmail));
        }
    }
}
