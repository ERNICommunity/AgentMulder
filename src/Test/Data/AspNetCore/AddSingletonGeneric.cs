// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGeneric
    {
        public AddSingletonGeneric()
        {
            var container = new ServiceCollection();
            container.AddSingleton<Foo>();
        }
    }
}