// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric1Param: ICompositionRoot
    {
        public RegisterGeneric1Param()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IBar, Bar>(new PerScopeLifetime());
        }
    }
}