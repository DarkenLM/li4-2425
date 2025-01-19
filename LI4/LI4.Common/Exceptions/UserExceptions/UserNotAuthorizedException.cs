namespace LI4.Common.Exceptions.UserExceptions {

    /// <summary>
    /// Exception thrown when a user is not authorized to perform an action.
    /// This exception is typically used when a user attempts to access a resource or 
    /// perform an operation they do not have the necessary permissions for.
    /// </summary>
    public class UserNotAuthorizedException : Exception {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthorizedException"/> class 
        /// with a default message indicating that the user is not authorized.
        /// </summary>
        public UserNotAuthorizedException() : base("User not authorized.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthorizedException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserNotAuthorizedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotAuthorizedException"/> class 
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UserNotAuthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
