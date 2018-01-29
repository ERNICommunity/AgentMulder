// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericFromToLambda
    {
        public AddSingletonGenericFromToLambda()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo, Foo>(s => new Foo());
        }
    }
}