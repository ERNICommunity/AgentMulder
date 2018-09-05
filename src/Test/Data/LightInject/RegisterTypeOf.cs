// Patterns: 2
// Matches: Foo.cs,Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterTypeOf: ICompositionRoot
    {
        public RegisterTypeOf()
        {
            var container = new ServiceContainer();
            container.Register(typeof(IFoo), typeof(Foo), "Foo", new PerScopeLifetime());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register(typeof(IBar), typeof(Bar), "Bar", new PerScopeLifetime());
        }
    }
}