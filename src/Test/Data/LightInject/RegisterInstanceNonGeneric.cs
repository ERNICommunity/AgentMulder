// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterInstanceNonGeneric: ICompositionRoot
    {
        public RegisterInstanceNonGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(typeof(Foo), new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterInstance(typeof(Bar), new Bar());
        }
    }
}