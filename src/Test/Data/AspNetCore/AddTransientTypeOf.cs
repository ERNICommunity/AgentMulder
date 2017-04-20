// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientTypeOf
    {
        public AddTransientTypeOf()
        {
            var container = new ServiceCollection();
            container.AddTransient(typeof(Foo));
        }
    }
}