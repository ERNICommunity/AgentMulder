// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericStatementLambdaSimple
    {
        public AddSingletonGenericStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo>(provider =>
                { return new Foo(); });
        }
    }
}