// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientTypeOfExpressionLambda
    {
        public AddTransientTypeOfExpressionLambda()
        {
            var container = new ServiceCollection();
            container.AddTransient(typeof(IFoo), provider => new Foo());
        }
    }
}