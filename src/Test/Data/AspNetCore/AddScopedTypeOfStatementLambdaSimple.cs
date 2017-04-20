// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedTypeOfStatementLambdaSimple
    {
        public AddScopedTypeOfStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddScoped(typeof(IFoo), provider =>
                { return new Foo(); });
        }
    }
}