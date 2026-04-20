using Microsoft.Extensions.Configuration;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ONGES.Users.Test.Infrastructure.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _service;
        private readonly string _key;

        public JwtTokenServiceTests()
        {
            _key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = _key,
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                    ["Jwt:ExpiresMinutes"] = "60"
                })
                .Build();

            _service = new JwtTokenService(config);
        }

        private static User CreateValidUser()
            => User.Create("Test User", Email.Create("test@email.com"), "Senha@123", EProfileType.Doador);

        [Fact]
        public void CreateToken_ShouldReturnValidToken()
        {
            var user = CreateValidUser();

            var tokenInfo = _service.CreateToken(user);

            Assert.NotNull(tokenInfo);
            Assert.NotEmpty(tokenInfo.Token);
            Assert.True(tokenInfo.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_ShouldContainExpectedClaims()
        {
            var user = CreateValidUser();

            var tokenInfo = _service.CreateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenInfo.Token);

            Assert.Equal("test@email.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == "UserId").Value);
            Assert.Equal("Test User", jwt.Claims.First(c => c.Type == "Name").Value);
            Assert.Equal("Doador", jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public void CreateToken_ShouldHaveCorrectIssuerAndAudience()
        {
            var user = CreateValidUser();

            var tokenInfo = _service.CreateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenInfo.Token);

            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Contains("TestAudience", jwt.Audiences);
        }

        [Fact]
        public void CreateToken_ShouldThrow_WhenKeyIsMissing()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                    ["Jwt:ExpiresMinutes"] = "60"
                })
                .Build();

            var service = new JwtTokenService(config);
            var user = CreateValidUser();

            Assert.Throws<InvalidOperationException>(() => service.CreateToken(user));
        }

        [Fact]
        public void CreateToken_ExpiresAt_ShouldMatchConfiguration()
        {
            var user = CreateValidUser();
            var before = DateTime.UtcNow.AddMinutes(60);

            var tokenInfo = _service.CreateToken(user);

            Assert.True(tokenInfo.ExpiresAt <= before.AddSeconds(5));
            Assert.True(tokenInfo.ExpiresAt >= before.AddSeconds(-5));
        }
    }
}
