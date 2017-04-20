// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedGenericStatementLambdaSimple
    {
        public AddScopedGenericStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddScoped<IFoo>(provider =>
                { return new Foo(); });
        }
    }
}