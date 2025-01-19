namespace LI4.Common.Exceptions.UserExceptions {

    /// <summary>
    /// Exception thrown when a user cannot be found in the system.
    /// This exception is typically used when authentication fails or 
    /// when an attempt to retrieve a user by email or ID results in no match.
    /// </summary>
    public class UserNotFoundException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class 
        /// with a default message indicating that the user was not found.
        /// </summary>
        public UserNotFoundException() : base("User not found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class
        /// with a specified error message
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
