﻿// <autogenerated />

namespace Piral.Blazor.Sdk;

[AttributeUsage(AttributeTargets.Assembly)]
public class AppShellAttribute : Attribute
{
    public AppShellAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
