// Patterns: 2
// Matches: Foo.cs, Baz.cs
// NotMatches: FooBar.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddScopedTypeOfStatementLambdaMultiple
    {
        public AddScopedTypeOfStatementLambdaMultiple()
        {
            var container = new ServiceCollection();
            container.AddScoped(typeof(IFoo), provider =>
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