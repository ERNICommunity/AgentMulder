// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterInstanceNonGeneric
    {
        public RegisterInstanceNonGeneric()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(typeof(Foo), new Foo());
        }
    }
}