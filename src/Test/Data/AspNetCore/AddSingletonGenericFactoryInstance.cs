// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddSingletonGenericFactoryInstance
    {
        public AddSingletonGenericFactoryInstance()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IFoo>(Create());
        }

        private Foo Create()
        {
            return new Foo();
        }
    }
}