
namespace LI4.Controllers;

[Serializable]
internal class UserHasNotEnoughBlocksException : Exception {
    public UserHasNotEnoughBlocksException() {
    }

    public UserHasNotEnoughBlocksException(string? message) : base(message) {
    }

    public UserHasNotEnoughBlocksException(string? message, Exception? innerException) : base(message, innerException) {
    }
}