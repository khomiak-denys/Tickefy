using System.Reflection;
using System.Runtime.CompilerServices;
using Tickefy.Application.Abstractions.Messaging;
using Shouldly;

namespace Tickefy.Architecture.Tests;

public class ImmutabilityTests : BaseTest
{
    [Fact]
    public void Commands_ShouldBeImmutable()
    {
        var commandTypes = ApplicationAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => IsCommandType(t));

        foreach (var type in commandTypes)
        {
            var mutableProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(IsMutableProperty)
                .Select(p => p.Name)
                .ToList();

            mutableProperties.ShouldBeEmpty($"Command '{type.Name}' has mutable properties: {string.Join(", ", mutableProperties)}");
        }
    }

    [Fact]
    public void Queries_ShouldBeImmutable()
    {
        var queryTypes = ApplicationAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => IsQueryType(t));

        foreach (var type in queryTypes)
        {
            var mutableProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(IsMutableProperty)
                .Select(p => p.Name)
                .ToList();

            mutableProperties.ShouldBeEmpty($"Query '{type.Name}' has mutable properties: {string.Join(", ", mutableProperties)}");
        }
    }

    private static bool IsCommandType(Type type)
    {
        return type.GetInterfaces().Any(i =>
            i == typeof(ICommand) ||
            (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));
    }

    private static bool IsQueryType(Type type)
    {
        return type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));
    }

    private static bool IsMutableProperty(PropertyInfo prop)
    {
        var setMethod = prop.SetMethod;
        if (setMethod is null || !setMethod.IsPublic)
        {
            return false;
        }

        var isInitOnly = setMethod.ReturnParameter.GetRequiredCustomModifiers()
            .Any(m => m == typeof(IsExternalInit));

        return !isInitOnly;
    }
}
