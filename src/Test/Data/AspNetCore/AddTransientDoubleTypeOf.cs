// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientDoubleTypeOf
    {
        public AddTransientDoubleTypeOf()
        {
            var container = new ServiceCollection();
            container.AddTransient(typeof(IFoo), typeof(Foo));
        }
    }
}