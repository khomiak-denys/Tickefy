using ArchUnitNET.xUnit;
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
}
