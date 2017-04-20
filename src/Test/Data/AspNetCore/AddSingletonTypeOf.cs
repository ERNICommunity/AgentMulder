// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonTypeOf
    {
        public AddSingletonTypeOf()
        {
            var container = new ServiceCollection();
            container.AddSingleton(typeof(Foo));
        }
    }
}