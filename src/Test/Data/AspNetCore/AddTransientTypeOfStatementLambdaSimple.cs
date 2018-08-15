// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientTypeOfStatementLambdaSimple
    {
        public AddTransientTypeOfStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddTransient(typeof(IFoo), provider =>
                { return new Foo(); });
        }
    }
}