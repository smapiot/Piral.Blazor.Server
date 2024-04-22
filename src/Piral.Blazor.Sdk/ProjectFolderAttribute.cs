namespace Piral.Blazor.Sdk;

[AttributeUsage(AttributeTargets.Assembly)]
public class ProjectFolderAttribute : Attribute
{
    public ProjectFolderAttribute(string path)
    {
        Path = path;
    }

    public string Path { get; }
}
