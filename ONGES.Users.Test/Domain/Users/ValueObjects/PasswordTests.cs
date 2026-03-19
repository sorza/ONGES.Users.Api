using ONGES.Users.Domain.Users.Exceptions.Password;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Test.Domain.Users.ValueObjects
{
    public class PasswordTests
    {
        [Fact]
        public void Create_ShouldReturnPassword_WhenValidPlainPassword()
        {
            // Arrange
            var plainPassword = "Valid1!#";

            // Act
            var password = Password.Create(plainPassword);

            // Assert
            Assert.NotNull(password);
            Assert.NotEmpty(password.Hash);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenPlainPasswordIsNullOrEmpty()
        {
            // Arrange
            string plainPassword = null!;

            // Act & Assert
            Assert.Throws<NullOrEmptyPasswordException>(() => Password.Create(plainPassword));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenPlainPasswordIsTooShort()
        {
            // Arrange
            var plainPassword = "short";

            // Act & Assert
            Assert.Throws<InvalidPasswordException>(() => Password.Create(plainPassword));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenPlainPasswordIsTooLong()
        {
            // Arrange
            var plainPassword = new string('a', 51) + "1!#";

            // Act & Assert
            Assert.Throws<InvalidPasswordException>(() => Password.Create(plainPassword));
        }

    }
}
