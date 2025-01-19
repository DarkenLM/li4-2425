namespace LI4.Common.Exceptions.ConstructionExceptions {

    /// <summary>
    /// Exception thrown when a construction could not be found in the system.
    /// This exception is typically used when attempting to retrieve or manipulate a construction 
    /// that does not exist or has been removed.
    /// </summary>
    public class ConstructionNotFoundException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionNotFoundException"/> class 
        /// with a default message indicating the construction was not found.
        /// </summary>
        public ConstructionNotFoundException() : base("Construction not found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionNotFoundException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ConstructionNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionNotFoundException"/> class 
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConstructionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
