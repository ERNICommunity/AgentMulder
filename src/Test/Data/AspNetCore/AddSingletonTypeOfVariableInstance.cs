// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonTypeOfVariableInstance
    {
        public AddSingletonTypeOfVariableInstance()
        {
            var instance = new Foo();
            var container = new ServiceCollection();
            container.AddSingleton(typeof(IFoo), instance);
        }
    }
}