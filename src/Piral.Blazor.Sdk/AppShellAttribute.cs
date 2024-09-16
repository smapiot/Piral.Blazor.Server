namespace Piral.Blazor.Sdk;

[AttributeUsage(AttributeTargets.Assembly)]
public class AppShellAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
