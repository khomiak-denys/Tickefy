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
    public void Validators_ShouldHave_NameEndingWith_Validator()
    {
        Classes().That()
            .HaveNameEndingWith("Validator")
            .Should().ResideInAssembly(ApplicationAssembly)
            .Check(Architecture);
    }
}
