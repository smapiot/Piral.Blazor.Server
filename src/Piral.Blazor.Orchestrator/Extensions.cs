using System.Reflection;
using System.Runtime.InteropServices;

namespace Piral.Blazor.Orchestrator;

internal static class Extensions
{
    public static (string, string) GetIdentity(this string id)
    {
        var idx = id.LastIndexOf('@');
        var front = id[..idx];
        var back = id[(idx + 1)..];
        return (front, back);
    }

    public static string MakePackageId(this NugetEntry entry) => $"{entry.Name}@{entry.Version}";

    public static IEnumerable<Type> GetTypesWithAttributes(this Assembly assembly, IReadOnlyCollection<Type> attributeTypes) =>
		assembly?.GetTypes().Where(m => m.HasAnyAttribute(attributeTypes)) ?? Enumerable.Empty<Type>();

	public static bool HasAnyAttribute(this Type member, IEnumerable<Type> attributeTypes) =>
		attributeTypes.Any(attributeType => Attribute.IsDefined(member, attributeType));

	public static IEnumerable<PropertyInfo> GetPropertiesIncludingInherited(this Type type, BindingFlags bindingFlags)
	{
		var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);

		Type? currentType = type;

		while (currentType != null)
		{
			var properties = currentType.GetProperties(bindingFlags | BindingFlags.DeclaredOnly);

			foreach (var property in properties)
			{
				if (!dictionary.TryGetValue(property.Name, out var others))
				{
					dictionary.Add(property.Name, property);
				}
				else if (!IsInheritedProperty(property, others))
				{
					List<PropertyInfo> many;

					if (others is PropertyInfo single)
					{
						many = new List<PropertyInfo> { single };
						dictionary[property.Name] = many;
					}
					else
					{
						many = (List<PropertyInfo>)others;
					}

					many.Add(property);
				}
			}

			currentType = currentType.BaseType;
		}

		foreach (var item in dictionary)
		{
			if (item.Value is PropertyInfo property)
			{
				yield return property;
				continue;
			}

			var list = (List<PropertyInfo>)item.Value;
			var count = list.Count;

			for (var i = 0; i < count; i++)
			{
				yield return list[i];
			}
		}
	}

	private static bool IsInheritedProperty(PropertyInfo property, object others)
	{
		if (others is PropertyInfo single)
		{
			return single.GetMethod?.GetBaseDefinition() == property.GetMethod?.GetBaseDefinition();
		}

		var many = (List<PropertyInfo>)others;

		foreach (var other in CollectionsMarshal.AsSpan(many))
		{
			if (other.GetMethod?.GetBaseDefinition() == property.GetMethod?.GetBaseDefinition())
			{
				return true;
			}
		}

		return false;
	}
}
