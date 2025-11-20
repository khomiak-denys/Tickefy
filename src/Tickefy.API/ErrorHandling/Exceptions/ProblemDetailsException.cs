namespace Tickefy.API.ErrorHandling.Exceptions
{
    public abstract class ProblemDetailsException : Exception
    {
        public abstract string Type { get; }
        public abstract string Title { get; }
        public abstract int Status { get; }

        public ProblemDetailsException(string message) : base(message) { }
    }
}