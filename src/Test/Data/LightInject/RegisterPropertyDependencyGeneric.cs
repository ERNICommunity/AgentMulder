// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterPropertyDependencyGeneric
    {
        public RegisterPropertyDependencyGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterPropertyDependency<IFoo>((factory, info) => new Foo());
        }
    }
}
