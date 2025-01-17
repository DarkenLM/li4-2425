namespace LI4.Common.Exceptions.ConstructionExceptions {

    public class UserHasNotEnoughBlocksException : Exception {
        public UserHasNotEnoughBlocksException() : base("Not enough blocks.") {
        }

        public UserHasNotEnoughBlocksException(string? message) : base(message) {
        }

        public UserHasNotEnoughBlocksException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}