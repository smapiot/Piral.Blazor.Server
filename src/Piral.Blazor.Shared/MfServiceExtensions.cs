namespace Piral.Blazor.Shared
{
    public static class MfServiceExtensions
    {
        /// <summary>
        /// Transforms the given relative local URL to a global micro frontend URL,
        /// e.g., for "my-image.jpg" it would return "/assets/[mf-name]/my-image.jpg".
        /// </summary>
        /// <param name="service">The micro frontend to convert the path for.</param>
        /// <param name="path">The path to transform.</param>
        /// <returns>The transformed path.</returns>
        public static string GetLink(this IMfService service, string path)
        {
            if (path.StartsWith("http:") || path.StartsWith("https:"))
            {
                return path;
            }
            else if (path.StartsWith("/"))
            {
                return service.GetLink(path.Substring(1));
            }
            else
            {
                var name = service.Meta.Name;
                return $"/assets/{name}/{path}";
            }
        }
    }
}
