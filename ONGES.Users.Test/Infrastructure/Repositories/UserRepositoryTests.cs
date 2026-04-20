using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Data;
using ONGES.Users.Infrastructure.Repositories;

namespace ONGES.Users.Test.Infrastructure.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new UserRepository(_context);
        }

        private static User CreateValidUser(string email = "auth@email.com")
            => User.Create("Auth User", Email.Create(email), "Senha@123", EProfileType.Doador);

        [Fact]
        public async Task Auth_ShouldReturnUser_WhenEmailExists()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            var email = Email.Create("auth@email.com");
            var result = await _repository.Auth(email, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task Auth_ShouldReturnNull_WhenEmailNotExists()
        {
            var email = Email.Create("notfound@email.com");
            var result = await _repository.Auth(email, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task Auth_ShouldReturnUntrackedEntity()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            var email = Email.Create("auth@email.com");
            var result = await _repository.Auth(email, CancellationToken.None);

            var entry = _context.Entry(result!);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
