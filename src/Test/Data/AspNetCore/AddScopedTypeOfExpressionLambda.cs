// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedTypeOfExpressionLambda
    {
        public AddScopedTypeOfExpressionLambda()
        {
            var container = new ServiceCollection();
            container.AddScoped(typeof(IFoo), provider => new Foo());
        }
    }
}