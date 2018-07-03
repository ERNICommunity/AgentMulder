// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientGeneric
    {
        public AddTransientGeneric()
        {
            var container = new ServiceCollection();
            container.AddTransient<Foo>();
        }
    }
}