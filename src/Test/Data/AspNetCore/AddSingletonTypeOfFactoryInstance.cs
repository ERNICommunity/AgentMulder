// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonTypeOfFactoryInstance
    {
        public AddSingletonTypeOfFactoryInstance()
        {
            var container = new ServiceCollection();
            container.AddSingleton(typeof(IFoo), Create());
        }

        private Foo Create()
        {
            return new Foo();
        }
    }
}