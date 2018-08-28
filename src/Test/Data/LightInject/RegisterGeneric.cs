// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric
    {
        public RegisterGeneric()
        {
            var container = new ServiceContainer();
            container.Register<Foo>();
        }
    }
}