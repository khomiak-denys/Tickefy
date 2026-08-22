using ArchUnitNET.Loader;
using Tickefy.API;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Tickets;
using Tickefy.Infrastructure.Database;
using Assembly = System.Reflection.Assembly;

namespace Tickefy.Architecture.Tests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Ticket).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(AppDbContext).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    protected static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            DomainAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly)
        .Build();
}
