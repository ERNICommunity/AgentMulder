// Patterns: 2
// Matches: Foo.cs,Baz.cs,Bar.cs
// NotMatches: FooBar.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterOrdered: ICompositionRoot
    {
        public RegisterOrdered()
        {
            var container = new ServiceContainer();
            container.RegisterOrdered(typeof(IFoo), new[] { typeof(Foo), typeof(Baz) }, type => new PerContainerLifetime());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterOrdered(typeof(IBar), new[] { typeof(Bar), typeof(Baz) }, type => new PerContainerLifetime());

        }
    }
}