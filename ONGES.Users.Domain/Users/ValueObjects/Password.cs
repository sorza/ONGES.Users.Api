using ONGES.Users.Domain.Shared.ValueObjects;
using ONGES.Users.Domain.Users.Exceptions;
using ONGES.Users.Domain.Users.Exceptions.Password;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ONGES.Users.Domain.Users.ValueObjects
{
    public sealed partial record Password : ValueObject
    {
        #region Constants

        public const int MaxLength = 50;
        public const int MinLength = 8;
        public const string Pattern = @"^(?=.{8,50}$)(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).*$";

        #endregion

        #region Properties     
        public string Hash { get; private set; } = string.Empty;
        #endregion

        #region Construtors

        private Password()
        {

        }

        private Password(string hash) => Hash = hash;

        #endregion

        #region Factory Method
        public static Password Create(string plain)
        {
            if (string.IsNullOrWhiteSpace(plain) || plain == string.Empty)
                throw new NullOrEmptyPasswordException(ErrorMessage.Password.NullOrEmpty);

            if (plain.Length is < MinLength or > MaxLength)
                throw new InvalidPasswordException(ErrorMessage.Password.Invalid);

            if (!SenhaRegex().IsMatch(plain))
                throw new InvalidPasswordException(ErrorMessage.Password.Invalid);

            var salt = RandomNumberGenerator.GetBytes(16);
            var hashed = Rfc2898DeriveBytes.Pbkdf2(
                                plain,
                                salt,
                                iterations: 100_000,
                                HashAlgorithmName.SHA256,
                                outputLength: 32);

            var result = Convert.ToBase64String(salt.Concat(hashed).ToArray());

            return new Password(result);

        }

        #endregion

        #region Methods
        public bool Verify(string password)
        {
            var data = Convert.FromBase64String(Hash);
            var salt = data[..16];
            var storedHash = data[16..];

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations: 100_000,
                HashAlgorithmName.SHA256,
                outputLength: 32);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);

        }

        #endregion

        #region Operators

        public static implicit operator string(Password senha) => senha.ToString();


        #endregion

        #region Overrides

        public override string ToString() => Hash;

        #endregion

        #region Others
        [GeneratedRegex(Pattern)]
        private static partial Regex SenhaRegex();

        #endregion
    }
}
