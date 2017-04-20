// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericExpressionLambda
    {
        public AddSingletonGenericExpressionLambda()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo>(provider => new Foo());
        }
    }
}