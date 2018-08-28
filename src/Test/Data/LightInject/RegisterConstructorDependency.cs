// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterConstructorDependency
    {
        public RegisterConstructorDependency()
        {
            var container = new ServiceContainer();
            container.RegisterConstructorDependency<IFoo>((factory, parameterInfo) => new Foo());
        }
    }
}