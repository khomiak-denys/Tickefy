namespace Tickefy.Domain.Common.Errors;

public class AlreadyExistsError(string message) : Error(message) { }