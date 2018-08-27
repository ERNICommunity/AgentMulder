// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric2
    {
        public RegisterGeneric2()
        {
            var container = new ServiceContainer();
            container.Register<IFoo,Foo>();
        }
    }
}