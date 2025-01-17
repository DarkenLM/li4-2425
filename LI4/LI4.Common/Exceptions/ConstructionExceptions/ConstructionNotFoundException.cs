namespace LI4.Common.Exceptions.ConstructionExceptions {

    public class ConstructionNotFoundException : Exception {

        public ConstructionNotFoundException() : base("Construction not found.") { }

        public ConstructionNotFoundException(string message) : base(message) { }

        // Constructor with a custom message and inner exception
        public ConstructionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
