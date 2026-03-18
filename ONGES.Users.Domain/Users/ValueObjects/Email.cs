using ONGES.Users.Domain.Shared.ValueObjects;
using ONGES.Users.Domain.Users.Exceptions;
using ONGES.Users.Domain.Users.Exceptions.Email;
using System.Text.RegularExpressions;

namespace ONGES.Users.Domain.Users.ValueObjects
{
    public sealed partial record Email : ValueObject
    {
        #region Constants

        public const int MaxLength = 160;
        public const int MinLength = 6;
        public const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        #endregion

        #region Properties
        public string Address { get; private set; } = string.Empty;
        #endregion

        #region Constructors
        private Email()
        {

        }

        private Email(string address)
        {
            Address = address;
        }
        #endregion

        #region Factories

        public static Email Create(string address)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
                throw new NullOrEmptyEmailException(ErrorMessage.Email.NullOrEmpty);

            address = address.Trim();
            address = address.ToLower();

            if (!EmailRegex().IsMatch(address))
                throw new InvalidEmailException(ErrorMessage.Email.Invalid);

            return new Email(address);

        }

        #endregion

        #region Operators

        public static implicit operator string(Email email) => email.ToString();


        #endregion

        #region Sobrecargas

        public override string ToString() => Address;

        #endregion

        #region Outros

        [GeneratedRegex(Pattern)]
        private static partial Regex EmailRegex();

        #endregion
    }
}
