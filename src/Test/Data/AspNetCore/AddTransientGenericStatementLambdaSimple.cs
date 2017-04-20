// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddTransientGenericStatementLambdaSimple
    {
        public AddTransientGenericStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddTransient<IFoo>(provider =>
                { return new Foo(); });
        }
    }
}