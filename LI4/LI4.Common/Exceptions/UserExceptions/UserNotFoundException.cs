namespace LI4.Common.Exceptions.UserExceptions {
    public class UserNotFoundException : Exception {

        public UserNotFoundException() : base("User not found.") { }

        public UserNotFoundException(string message) : base(message) { }

        // Constructor with a custom message and inner exception
        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserNotAuthorizedException : Exception {
        public UserNotAuthorizedException() : base("User not authorized.") { }

        public UserNotAuthorizedException(string message) : base(message) { }

        // Constructor with a custom message and inner exception
        public UserNotAuthorizedException(string message, Exception innerException) : base(message, innerException) { }

    }
}
