// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterInstanceGeneric: ICompositionRoot
    {
        public RegisterInstanceGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterInstance<Foo>(new Foo(), "Foo");
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterInstance<Bar>(new Bar(), "Bar");
        }
    }
}