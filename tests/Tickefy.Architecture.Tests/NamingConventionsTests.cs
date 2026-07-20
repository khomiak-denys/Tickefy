using System.Reflection;
using ArchUnitNET.xUnit;
using Shouldly;
using Tickefy.Application.Abstractions.Messaging;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Tickefy.Architecture.Tests;

public class NamingConventionsTests : BaseTest
{
    [Fact]
    public void Commands_ShouldHave_NameEndingWith_Command()
    {
        Classes().That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should().HaveNameEndingWith("Command")
            .Check(Architecture);
    }

    [Fact]
    public void Queries_ShouldHave_NameEndingWith_Query()
    {
        Classes().That()
            .ImplementInterface(typeof(IQuery<>))
            .Should().HaveNameEndingWith("Query")
            .Check(Architecture);
    }

    [Fact]
    public void CommandHandlers_ShouldHave_NameEndingWith_CommandHandler()
    {
        Classes().That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should().HaveNameEndingWith("CommandHandler")
            .Check(Architecture);
    }

    [Fact]
    public void QueryHandlers_ShouldHave_NameEndingWith_QueryHandler()
    {
        Classes().That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should().HaveNameEndingWith("QueryHandler")
            .Check(Architecture);
    }

    [Fact]
    public void AbstractValidators_ShouldHave_NameEndingWith_Validator()
    {
        Classes().That()
            .AreAssignableTo(typeof(FluentValidation.AbstractValidator<>))
            .Should().HaveNameEndingWith("Validator")
            .Check(Architecture);
    }

    [Fact]
    public void AbstractValidators_ShouldResideIn_ApplicationAssembly()
    {
        Classes().That()
            .AreAssignableTo(typeof(FluentValidation.AbstractValidator<>))
            .Should().ResideInAssembly(ApplicationAssembly)
            .Check(Architecture);
    }

    [Fact]
    public void Interfaces_ShouldHave_NameStartingWith_I()
    {
        Interfaces().That()
            .ResideInAssembly(DomainAssembly)
            .Or().ResideInAssembly(ApplicationAssembly)
            .Or().ResideInAssembly(InfrastructureAssembly)
            .Or().ResideInAssembly(ApiAssembly)
            .Should().HaveNameStartingWith("I")
            .Check(Architecture);
    }

    [Fact]
    public void AsyncMethods_ShouldHave_NameEndingWith_Async()
    {
        Assembly[] assemblies = [DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly];

        var nonAsyncNamedMethods = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true } or { IsInterface: true })
            .Where(t => !typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(t))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(m => !m.IsSpecialName) // Exclude property getters/setters and events
            .Where(m => typeof(Task).IsAssignableFrom(m.ReturnType) || (m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>)) || m.ReturnType == typeof(ValueTask))
            .Where(m => m.Name != "Handle") // Skip MediatR Handle method required by interface contract
            .Where(m => !m.Name.EndsWith("Async", StringComparison.Ordinal))
            .Select(m => $"{m.DeclaringType?.FullName}.{m.Name}")
            .Distinct()
            .ToList();

        nonAsyncNamedMethods.ShouldBeEmpty($"Async methods returning Task or ValueTask should end with 'Async'. Violations: {string.Join(", ", nonAsyncNamedMethods)}");
    }
}
