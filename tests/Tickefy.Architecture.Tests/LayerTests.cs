using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Tickefy.Architecture.Tests;

public class LayerTests : BaseTest
{
    private static readonly IObjectProvider<IType> DomainLayer =
        Types().That().ResideInAssembly(DomainAssembly).As("Domain Layer");

    private static readonly IObjectProvider<IType> ApplicationLayer =
        Types().That().ResideInAssembly(ApplicationAssembly).As("Application Layer");

    private static readonly IObjectProvider<IType> InfrastructureLayer =
        Types().That().ResideInAssembly(InfrastructureAssembly).As("Infrastructure Layer");

    private static readonly IObjectProvider<IType> ApiLayer =
        Types().That().ResideInAssembly(ApiAssembly).As("API Layer");

    [Fact]
    public void DomainLayer_ShouldNotDependOn_ApplicationLayer()
    {
        Types().That().Are(DomainLayer).Should()
            .NotDependOnAny(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_ShouldNotDependOn_InfrastructureLayer()
    {
        Types().That().Are(DomainLayer).Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_ShouldNotDependOn_ApiLayer()
    {
        Types().That().Are(DomainLayer).Should()
            .NotDependOnAny(ApiLayer)
            .Check(Architecture);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotDependOn_InfrastructureLayer()
    {
        Types().That().Are(ApplicationLayer).Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void ApplicationLayer_ShouldNotDependOn_ApiLayer()
    {
        Types().That().Are(ApplicationLayer).Should()
            .NotDependOnAny(ApiLayer)
            .Check(Architecture);
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotDependOn_ApiLayer()
    {
        Types().That().Are(InfrastructureLayer).Should()
            .NotDependOnAny(ApiLayer)
            .Check(Architecture);
    }

    [Fact]
    public void AppDbContext_ShouldNotBeUsedIn_ApplicationLayer()
    {
        Types().That().Are(ApplicationLayer).Should()
            .NotDependOnAny(typeof(Tickefy.Infrastructure.Database.AppDbContext))
            .Check(Architecture);
    }

    [Fact]
    public void AppDbContext_ShouldNotBeUsedIn_Controllers()
    {
        Classes().That().AreAssignableTo(typeof(Microsoft.AspNetCore.Mvc.ControllerBase)).Should()
            .NotDependOnAny(typeof(Tickefy.Infrastructure.Database.AppDbContext))
            .Check(Architecture);
    }

    [Fact]
    public void RepositoryImplementations_ShouldResideIn_InfrastructureLayer_And_HaveNameEndingWith_Repository()
    {
        var domainRepositories = Types().That().HaveNameEndingWith("Repository").And().ResideInAssembly(DomainAssembly);

        Classes().That().AreAssignableTo(domainRepositories)
            .Should().ResideInAssembly(InfrastructureAssembly)
            .AndShould().HaveNameEndingWith("Repository")
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_ShouldOnlyDependOn_AllowedNamespaces()
    {
        var allowedDependencies = Types().That()
            .ResideInNamespaceMatching(@"^Tickefy\.Domain(\..*)?$")
            .Or()
            .ResideInNamespaceMatching(@"^System(\..*)?$")
            .As("Allowed Domain Dependencies");

        Types().That().Are(DomainLayer).Should()
            .OnlyDependOn(allowedDependencies)
            .Check(Architecture);
    }
}
