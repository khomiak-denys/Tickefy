namespace Tickefy.Domain.Common.Errors;

public class InvalidArgumentError(string message) : Error(message) { }