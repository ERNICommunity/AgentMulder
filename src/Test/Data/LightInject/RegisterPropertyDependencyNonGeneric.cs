// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterPropertyDependencyNonGeneric
    {
        public RegisterPropertyDependencyNonGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterPropertyDependency((factory, info) => new Foo());
        }
    }
}
