using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Infrastructure.Validators;

namespace ONGES.Users.Test.Infrastructure.Validators
{
    public class UserRequestValidatorTests
    {
        private readonly UserRequestValidator _validator = new();

        [Fact]
        public void ShouldPass_WhenRequestIsValid()
        {
            var request = new UserRequest("User Name", "user@email.com", "Senha@123");

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("", "user@email.com", "Senha@123")]
        [InlineData("AB", "user@email.com", "Senha@123")]
        public void ShouldFail_WhenNameIsInvalid(string name, string email, string password)
        {
            var request = new UserRequest(name, email, password);

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("User", "", "Senha@123")]
        [InlineData("User", "a@b", "Senha@123")]
        public void ShouldFail_WhenEmailIsInvalid(string name, string email, string password)
        {
            var request = new UserRequest(name, email, password);

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("User", "user@email.com", "")]
        [InlineData("User", "user@email.com", "short")]
        public void ShouldFail_WhenPasswordIsInvalid(string name, string email, string password)
        {
            var request = new UserRequest(name, email, password);

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }
    }
}
