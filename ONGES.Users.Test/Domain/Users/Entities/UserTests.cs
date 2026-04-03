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
        User ValidUser;
        string name = "Teste";
        Email email = Email.Create("teste@teste.com");
        string password = "P@ssw0rd1";
        EProfileType profile = EProfileType.Gestor;

        public UserTests()
        {
            ValidUser = User.Create(name, email, password, profile);
        }

        [Fact]
        public void CreateUser_ValidData_ShouldCreateUser()
        {           

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
            name = "";        

            // Act & Assert
            Assert.Throws<NullOrEmptyNameException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidProfile_ShouldThrowException()
        {
            // Arrange
            profile = (EProfileType)999;

            // Act & Assert
            Assert.Throws<InvalidProfileException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidPassword_ShouldThrowException()
        {
            // Arrange            
            password = "invalid";

            // Act & Assert
            Assert.Throws<InvalidPasswordException>(() => User.Create(name, email, password, profile));
        }

        [Fact]
        public void CreateUser_InvalidEmail_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<InvalidEmailException>(() => User.Create(name, Email.Create("email-invalido"), password, profile));
        }

        [Fact]
        public void UpdateRole_InvalidProfile_ShouldThrowException()
        {   
            Assert.Throws<InvalidProfileException>(() => ValidUser.UpdateRole((EProfileType)999));
        }

        [Fact]
        public void UpdateRole_ValidProfile_ShouldUpdateRole()
        {            
            ValidUser.UpdateRole(EProfileType.Doador);
            Assert.Equal(EProfileType.Doador, ValidUser.Profile);
        }
    
        [Fact]
        public void Activate_ShouldSetActiveToTrue()
        {
            ValidUser.Deactivate();
            ValidUser.Activate();
            Assert.True(ValidUser.Active);
        }

        [Fact]
        public void Deactivate_ShouldSetActiveToFalse()
        {
            ValidUser.Deactivate();
            Assert.False(ValidUser.Active);
        }
    }
}
