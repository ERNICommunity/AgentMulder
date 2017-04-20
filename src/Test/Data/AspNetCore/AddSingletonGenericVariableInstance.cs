// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericVariableInstance
    {
        public AddSingletonGenericVariableInstance()
        {
            var instance = new Foo();
            var container = new ServiceCollection();
            container.AddSingleton<IFoo>(instance);
        }
    }
}