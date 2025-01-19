namespace LI4.Common.Exceptions.UserExceptions {

    /// <summary>
    /// Exception thrown when attempting to create a user that already exists in the system.
    /// This exception is typically used when trying to register a new user with an email or username 
    /// that is already in use.
    /// </summary>
    public class UserAlreadyExistsException : Exception {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class 
        /// with a default message indicating that the user already exists.
        /// </summary>
        public UserAlreadyExistsException() : base("User already exists.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserAlreadyExistsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class 
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
