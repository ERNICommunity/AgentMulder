// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonTypeOfExpressionLambda
    {
        public AddSingletonTypeOfExpressionLambda()
        {
            var container = new ServiceCollection();
            container.AddSingleton(typeof(IFoo), provider => new Foo());
        }
    }
}