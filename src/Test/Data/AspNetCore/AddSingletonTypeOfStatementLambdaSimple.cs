// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonTypeOfStatementLambdaSimple
    {
        public AddSingletonTypeOfStatementLambdaSimple()
        {
            var container = new ServiceCollection();
            container.AddSingleton(typeof(IFoo), provider =>
                { return new Foo(); });
        }
    }
}