namespace LI4.Common.Exceptions.UserExceptions {

    public class UserAlreadyExistsException : Exception {
        public UserAlreadyExistsException() : base("User already exists.") { }

        public UserAlreadyExistsException(string message) : base(message) { }

        // Constructor with a custom message and inner exception
        public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
