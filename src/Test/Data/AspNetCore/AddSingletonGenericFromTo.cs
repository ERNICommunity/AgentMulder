// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericFromTo
    {
        public AddSingletonGenericFromTo()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo, Foo>();
        }
    }
}