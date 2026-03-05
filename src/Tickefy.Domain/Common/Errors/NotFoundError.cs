namespace Tickefy.Domain.Common.Errors;

public class NotFoundError(string message) : Error(message) { }