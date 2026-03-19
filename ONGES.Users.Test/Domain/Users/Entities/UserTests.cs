using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.Exceptions.Email;
using ONGES.Users.Domain.Users.Exceptions.Password;
using ONGES.Users.Domain.Users.Exceptions.Users;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Test.Domain.Users.Entities
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_ValidData_ShouldCreateUser()
        {
            // Arrange
            var name = "John Doe";
            var email = Email.Create("john.doe@example.com");
            var password = "P@ssw0rd1";
            var profile = EProfileType.Doador;

            //Act
            var user = User.Create(name, email, password, profile);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(name, user.Name);
            Assert.Equal(email, user.Email);
            Assert.True(user.Password.Verify(password));
            Assert.Equal(profile, user.Profile);
            Assert.True(user.Active);
        }

        [Fact]
        public void CreateUser_InvalidName_ShouldThrowException()
        {
            // Arrange
            var name = "";
            var email = Email.Create("john.doe@example.com");
            var password = "P@ssw0rd1";
            var profile = EProfileType.Doador;

            // Act & Assert
            Assert.Throws<NullOrEmptyNameException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidProfile_ShouldThrowException()
        {
            // Arrange
            var name = "John Doe";
            var email = Email.Create("john.doe@example.com");
            var password = "P@ssw0rd1";
            var profile = (EProfileType)999; 

            // Act & Assert
            Assert.Throws<InvalidProfileException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidPassword_ShouldThrowException()
        {
            // Arrange
            var name = "John Doe";
            var email = Email.Create("john.doe@example.com");
            var password = "invalid"; 
            var profile = EProfileType.Doador;

            // Act & Assert
            Assert.Throws<InvalidPasswordException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidEmail_ShouldThrowException()
        {
            // Arrange
            var name = "John Doe";
            var email = "invalid-email"; 
            var password = "P@ssw0rd1";
            var profile = EProfileType.Doador;

            // Act & Assert
            Assert.Throws<InvalidEmailException>(() => User.Create(name, Email.Create(email), password, profile));
        }
    }
}
