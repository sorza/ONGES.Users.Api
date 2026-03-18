using ONGES.Users.Domain.Shared.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.Exceptions;
using ONGES.Users.Domain.Users.Exceptions.Users;
using ONGES.Users.Domain.Users.ValueObjects;
using System.Security.Principal;

namespace ONGES.Users.Domain.Users.Entities
{
    public class User : Entity
    {
        #region Constructors
        private User(Guid id) : base(id)
        {
        }

        private User(Guid id, string name, Email email, Password password, EProfileType profileType) : base(id)
        {
            Name = name;
            Email = email;
            Password = password;
            Profile = profileType;
        }

        private User(Guid id, string name, string password, Email email, EProfileType profile) : base(id)
        {
            Name = name;
            Password = Password.CreateFromHash(password);
            Email = email;
            Profile = profile;
        }

        #endregion

        #region Properties
        public string Name { get; private set; } = string.Empty;
        public Password Password { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public EProfileType Profile { get; private set; }
        public bool Active { get; private set; } = true;

        #endregion

        #region Factories Methods

        public static User Create(string name, string password, string email, EProfileType profile)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NullOrEmptyNameException(ErrorMessage.User.NullOrEmpty);

            if (!Enum.IsDefined(typeof(EProfileType), profile))
                throw new InvalidProfileException(ErrorMessage.User.InvalidProfileType);

            var senha_result = Password.Create(password);
            var email_result = Email.Create(email);

            return new User(Guid.NewGuid(), name, senha_result, email_result, profile);
        }        

        #endregion

    }
}
