using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Data;
using ONGES.Users.Infrastructure.Repositories;

namespace ONGES.Users.Test.Infrastructure.Repositories
{
    public class GenericRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly AppDbContext _context;
        private readonly GenericRepository<User> _repository;

        public GenericRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new GenericRepository<User>(_context);
        }

        private static User CreateValidUser(string name = "Test User", string email = "test@email.com")
            => User.Create(name, Email.Create(email), "Senha@123", EProfileType.Doador);

        [Fact]
        public async Task CreateAsync_ShouldPersistUser()
        {
            var user = CreateValidUser();

            await _repository.CreateAsync(user);

            var result = await _context.Set<User>().FindAsync(user.Id);
            Assert.NotNull(result);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnUser_WhenExists()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAsync(u => u.Id == user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenNotExists()
        {
            var id = Guid.NewGuid();
            var result = await _repository.GetAsync(u => u.Id == id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredUsers()
        {
            var user1 = CreateValidUser("User One", "one@email.com");
            var user2 = CreateValidUser("User Two", "two@email.com");
            user2.Deactivate();

            _context.Set<User>().AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync(u => u.Active);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoMatch()
        {
            var result = await _repository.GetAllAsync(u => u.Active);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            var exists = await _repository.ExistsAsync(u => u.Email.Address == "test@email.com");

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
        {
            var exists = await _repository.ExistsAsync(u => u.Email.Address == "notfound@email.com");

            Assert.False(exists);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyUser()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();
            _context.Entry(user).State = EntityState.Detached;

            var tracked = await _context.Set<User>().FindAsync(user.Id);
            tracked!.Deactivate();
            await _repository.UpdateAsync(tracked);

            var updated = await _context.Set<User>().FindAsync(user.Id);
            Assert.False(updated!.Active);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser_WhenExists()
        {
            var user = CreateValidUser();
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(user.Id);

            var result = await _context.Set<User>().FindAsync(user.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenNotExists()
        {
            await _repository.DeleteAsync(Guid.NewGuid());

            Assert.Empty(_context.Set<User>());
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
