namespace Tickefy.Domain.Common.Errors;

public class ForbiddenError(string message) : Error(message) { }
