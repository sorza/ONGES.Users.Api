using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Infrastructure.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.OwnsOne(x => x.Email, email =>
            {
                email.Property(x => x.Address)
                .HasColumnName("Email")
                .HasColumnType("VARCHAR")
                .HasMaxLength(Email.MaxLength)
                .IsRequired(true);
            });

            builder.OwnsOne(x => x.Password, password =>
            {
                password.Property(s => s.Hash)
                .HasColumnName("Password")
                .HasMaxLength(256)
                .IsRequired(true);
            });

        }
    }
}
