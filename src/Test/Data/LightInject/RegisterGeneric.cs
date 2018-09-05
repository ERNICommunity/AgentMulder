// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric:ICompositionRoot
    {
        public RegisterGeneric()
        {
            var container = new ServiceContainer();
            container.Register<Foo>();
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<Bar>();
        }
    }
}