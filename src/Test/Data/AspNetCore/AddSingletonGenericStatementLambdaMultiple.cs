// Patterns: 2
// Matches: Foo.cs, Baz.cs
// NotMatches: FooBar.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericStatementLambdaMultiple
    {
        public AddSingletonGenericStatementLambdaMultiple()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo>(provider =>
            {
                if (new object() == null)
                {
                    return new Foo();
                }
                else
                {
                    return new Baz();
                }
            });
        }
    }
}