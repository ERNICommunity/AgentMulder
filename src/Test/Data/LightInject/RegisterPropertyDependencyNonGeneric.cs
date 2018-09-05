// Patterns: 2
// Matches: Foo.cs,Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterPropertyDependencyNonGeneric:ICompositionRoot
    {
        public RegisterPropertyDependencyNonGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterPropertyDependency((factory, info) => new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterPropertyDependency((factory, info) => new Bar());
        }
    }
}
