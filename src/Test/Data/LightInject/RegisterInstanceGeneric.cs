// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterInstanceGeneric
    {
        public RegisterInstanceGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterInstance<Foo>(new Foo(), "Foo");
        }
    }
}