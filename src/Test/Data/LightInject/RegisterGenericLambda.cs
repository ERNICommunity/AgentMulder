// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambda: ICompositionRoot
    {
        public RegisterGenericLambda()
        {
            var container = new ServiceContainer();
            container.Register<IFoo>(x => new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IBar>(x => new Bar());
        }
    }
}