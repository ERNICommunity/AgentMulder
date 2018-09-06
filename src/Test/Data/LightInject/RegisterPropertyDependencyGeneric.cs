// Patterns: 2
// Matches: Foo.cs,Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterPropertyDependencyGeneric: ICompositionRoot
    {
        public RegisterPropertyDependencyGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterPropertyDependency<IFoo>((factory, info) => new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterPropertyDependency<IBar>((factory, info) => new Bar());

        }
    }
}
