// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedGenericFromToLambda
    {
        public AddScopedGenericFromToLambda()
        {
            var container = new ServiceCollection();
            container.AddScoped<IFoo, Foo>(s => new Foo());
        }
    }
}