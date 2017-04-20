// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedGeneric
    {
        public AddScopedGeneric()
        {
            var container = new ServiceCollection();
            container.AddScoped<Foo>();
        }
    }
}