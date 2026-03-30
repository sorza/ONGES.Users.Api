using Moq;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Application.Services;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Services;

namespace ONGES.Users.Test.Application.Users
{
    public class ServicesTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IJwtTokenService> _jwtTokenService = new();

        private IUserService _userService;

        public ServicesTests()
        {
           _userService = new UserService(_userRepositoryMock.Object, _jwtTokenService.Object, null!, null!);
        }


        }
}
