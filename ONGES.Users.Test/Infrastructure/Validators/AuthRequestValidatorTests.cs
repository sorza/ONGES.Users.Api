using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Infrastructure.Validators;

namespace ONGES.Users.Test.Infrastructure.Validators
{
    public class AuthRequestValidatorTests
    {
        private readonly AuthRequestValidator _validator = new();

        [Fact]
        public void ShouldPass_WhenRequestIsValid()
        {
            var request = new AuthRequest("user@email.com", "Senha@123");

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("", "Senha@123")]
        [InlineData("invalid-email", "Senha@123")]
        public void ShouldFail_WhenEmailIsInvalid(string email, string password)
        {
            var request = new AuthRequest(email, password);

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("user@email.com", "")]
        [InlineData("user@email.com", "short")]
        public void ShouldFail_WhenPasswordIsInvalid(string email, string password)
        {
            var request = new AuthRequest(email, password);

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }
    }
}
