// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterTypeOf
    {
        public RegisterTypeOf()
        {
            var container = new ServiceContainer();
            container.Register(typeof(IFoo), typeof(Foo), "Foo", new PerScopeLifetime());
        }
    }
}