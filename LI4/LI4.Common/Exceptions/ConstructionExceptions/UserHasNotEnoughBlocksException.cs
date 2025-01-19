namespace LI4.Common.Exceptions.ConstructionExceptions {

    /// <summary>
    /// Exception thrown when a user does not have enough blocks to complete a construction.
    /// This exception is typically used when a user attempts to initiate a construction 
    /// but does not have the required number of blocks in their inventory.
    /// </summary>
    public class UserHasNotEnoughBlocksException : Exception {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserHasNotEnoughBlocksException"/> class 
        /// with a default message indicating the user does not have enough blocks.
        /// </summary>
        public UserHasNotEnoughBlocksException() : base("Not enough blocks.") {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHasNotEnoughBlocksException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserHasNotEnoughBlocksException(string? message) : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHasNotEnoughBlocksException"/> class 
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UserHasNotEnoughBlocksException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}