// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterConstructorDependency: ICompositionRoot
    {
        public RegisterConstructorDependency()
        {
            var container = new ServiceContainer();
            container.RegisterConstructorDependency<IFoo>((factory, parameterInfo) => new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterConstructorDependency<IBar>((factory, parameterInfo) => new Bar());
        }
    }
}