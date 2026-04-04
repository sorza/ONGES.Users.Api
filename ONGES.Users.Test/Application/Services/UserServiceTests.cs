using FluentValidation;
using FluentValidation.Results;
using Moq;
using ONGES.Users.Application.DTOs.Events;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.Events;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Application.Services;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Services;
using System.Linq.Expressions;

namespace ONGES.Users.Test.Application.UserServices
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IJwtTokenService> _jwtServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<IValidator<UserRequest>> _userValidatorMock;
        private readonly Mock<IValidator<AuthRequest>> _authValidatorMock;
        private readonly UserService _sut;

        private const string ValidName = "Teste";
        private const string ValidEmail = "teste@teste.com";
        private const string ValidPassword = "P@ssw0rd1";
        private const string CorrelationId = "test-correlation-id";

        public UserServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtTokenService>();
            _eventStoreMock = new Mock<IEventStore>();
            _userValidatorMock = new Mock<IValidator<UserRequest>>();
            _authValidatorMock = new Mock<IValidator<AuthRequest>>();

            _sut = new UserService(
                _repositoryMock.Object,
                _jwtServiceMock.Object,
                _eventStoreMock.Object,
                _userValidatorMock.Object,
                _authValidatorMock.Object);
        }

        private static User CreateValidUser()
            => User.Create(ValidName, Email.Create(ValidEmail), ValidPassword, EProfileType.Doador);

        private static ValidationResult ValidValidationResult()
            => new();

        private static ValidationResult InvalidValidationResult(string message = "Erro de validação")
            => new(new[] { new ValidationFailure("Field", message) });

        #region AddUserAsync

        [Fact]
        public async Task AddUserAsync_DeveRetornarSucesso_QuandoDadosValidos()
        {
            var request = new UserRequest(ValidName, ValidPassword, ValidEmail);
            _userValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.AddUserAsync(request, CorrelationId);

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidName, result.Value.Name);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _eventStoreMock.Verify(e => e.AppendAsync(It.IsAny<string>(), It.IsAny<UserCreatedEvent>(), 0, CorrelationId), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_DeveRetornarFalha_QuandoValidacaoFalha()
        {
            var request = new UserRequest("", ValidPassword, ValidEmail);
            _userValidatorMock.Setup(v => v.Validate(request)).Returns(InvalidValidationResult());

            var result = await _sut.AddUserAsync(request, CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("400", result.Error.Code);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _eventStoreMock.Verify(e => e.AppendAsync(It.IsAny<string>(), It.IsAny<UserCreatedEvent>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_DeveRetornarFalha_QuandoUsuarioJaExiste()
        {
            var request = new UserRequest(ValidName, ValidPassword, ValidEmail);
            _userValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.AddUserAsync(request, CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("409", result.Error.Code);
            _eventStoreMock.Verify(e => e.AppendAsync(It.IsAny<string>(), It.IsAny<UserCreatedEvent>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region AuthAsync

        [Fact]
        public async Task AuthAsync_DeveRetornarSucesso_QuandoCredenciaisValidas()
        {
            var request = new AuthRequest(ValidEmail, ValidPassword);
            var user = CreateValidUser();
            var tokenInfo = new TokenInfo("jwt-token", DateTime.UtcNow.AddHours(2));

            _authValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.Auth(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _jwtServiceMock.Setup(j => j.CreateToken(user)).Returns(tokenInfo);

            var result = await _sut.AuthAsync(request, "127.0.0.1", "TestAgent", CorrelationId);

            Assert.True(result.IsSuccess);
            Assert.Equal("jwt-token", result.Value.AccessToken);
        }

        [Fact]
        public async Task AuthAsync_DeveRetornarFalha_QuandoValidacaoFalha()
        {
            var request = new AuthRequest("", "");
            _authValidatorMock.Setup(v => v.Validate(request)).Returns(InvalidValidationResult());

            var result = await _sut.AuthAsync(request, "127.0.0.1", "TestAgent", CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("400", result.Error.Code);
        }

        [Fact]
        public async Task AuthAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            var request = new AuthRequest(ValidEmail, ValidPassword);
            _authValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.Auth(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.AuthAsync(request, "127.0.0.1", "TestAgent", CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("401", result.Error.Code);
        }

        [Fact]
        public async Task AuthAsync_DeveRetornarFalha_QuandoSenhaIncorreta()
        {
            var request = new AuthRequest(ValidEmail, "SenhaErrada1!");
            var user = CreateValidUser();

            _authValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.Auth(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.AuthAsync(request, "127.0.0.1", "TestAgent", CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("401", result.Error.Code);
        }

        [Fact]
        public async Task AuthAsync_DeveRetornarFalha_QuandoUsuarioInativo()
        {
            var request = new AuthRequest(ValidEmail, ValidPassword);
            var user = CreateValidUser();
            user.Deactivate();

            _authValidatorMock.Setup(v => v.Validate(request)).Returns(ValidValidationResult());
            _repositoryMock.Setup(r => r.Auth(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.AuthAsync(request, "127.0.0.1", "TestAgent", CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("403", result.Error.Code);
        }

        #endregion

        #region GetUserAsync

        [Fact]
        public async Task GetUserAsync_DeveRetornarSucesso_QuandoUsuarioEncontrado()
        {
            var user = CreateValidUser();
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.GetUserAsync(u => u.Id == user.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal(user.Id, result.Value.Id);
        }

        [Fact]
        public async Task GetUserAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.GetUserAsync(u => u.Id == Guid.NewGuid());

            Assert.True(result.IsFailure);
            Assert.Equal("404", result.Error.Code);
        }

        #endregion

        #region GetAllUsersAsync

        [Fact]
        public async Task GetAllUsersAsync_DeveRetornarSucesso_ComListaDeUsuarios()
        {
            var users = new List<User> { CreateValidUser() };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _sut.GetAllUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetAllUsersAsync_DeveRetornarSucesso_ComListaVazia()
        {
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<User>());

            var result = await _sut.GetAllUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

        #endregion

        #region GetActiveUsersAsync

        [Fact]
        public async Task GetActiveUsersAsync_DeveRetornarSucesso_ComUsuariosAtivos()
        {
            var users = new List<User> { CreateValidUser() };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var result = await _sut.GetActiveUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
        }

        #endregion

        #region RemoveUserAsync

        [Fact]
        public async Task RemoveUserAsync_DeveRetornarSucesso_QuandoUsuarioEncontrado()
        {
            var user = CreateValidUser();
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.RemoveUserAsync(user.Id, CorrelationId);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(r => r.DeleteAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.RemoveUserAsync(Guid.NewGuid(), CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("404", result.Error.Code);
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region DeactivateUserAsync

        [Fact]
        public async Task DeactivateUserAsync_DeveRetornarSucesso_QuandoUsuarioEncontrado()
        {
            var user = CreateValidUser();
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.DeactivateUserAsync(user.Id, CorrelationId);

            Assert.True(result.IsSuccess);
            Assert.False(user.Active);
            _repositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateUserAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.DeactivateUserAsync(Guid.NewGuid(), CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("404", result.Error.Code);
        }

        #endregion

        #region ActivateUserAsync

        [Fact]
        public async Task ActivateUserAsync_DeveRetornarSucesso_QuandoUsuarioEncontrado()
        {
            var user = CreateValidUser();
            user.Deactivate();
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.ActivateUserAsync(user.Id, CorrelationId);

            Assert.True(result.IsSuccess);
            Assert.True(user.Active);
            _repositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActivateUserAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.ActivateUserAsync(Guid.NewGuid(), CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("404", result.Error.Code);
        }

        #endregion

        #region UpdateRoleUserAsync

        [Fact]
        public async Task UpdateRoleUserAsync_DeveRetornarSucesso_QuandoUsuarioEncontrado()
        {
            var user = CreateValidUser();
            var request = new UpdateRoleRequest(user.Id, EProfileType.Gestor);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var result = await _sut.UpdateRoleUserAsync(request, CorrelationId);

            Assert.True(result.IsSuccess);
            Assert.Equal(EProfileType.Gestor, user.Profile);
            _repositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleUserAsync_DeveRetornarFalha_QuandoUsuarioNaoEncontrado()
        {
            var request = new UpdateRoleRequest(Guid.NewGuid(), EProfileType.Gestor);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var result = await _sut.UpdateRoleUserAsync(request, CorrelationId);

            Assert.True(result.IsFailure);
            Assert.Equal("404", result.Error.Code);
        }        

        #endregion
    }
}
