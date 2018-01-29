// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientGenericFromTo
    {
        public AddTransientGenericFromTo()
        {
            var container = new ServiceCollection();
            container.AddTransient<IFoo, Foo>();
        }
    }
}