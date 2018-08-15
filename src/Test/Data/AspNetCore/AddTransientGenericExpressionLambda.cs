// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientGenericExpressionLambda
    {
        public AddTransientGenericExpressionLambda()
        {
            var container = new ServiceCollection();
            container.AddTransient<IFoo>(provider => new Foo());
        }
    }
}