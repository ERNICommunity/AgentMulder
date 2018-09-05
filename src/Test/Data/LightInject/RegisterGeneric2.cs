// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric2: ICompositionRoot
    {
        public RegisterGeneric2()
        {
            var container = new ServiceContainer();
            container.Register<IFoo,Foo>();
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IBar, Bar>();
        }
    }
}