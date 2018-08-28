// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGeneric1Param
    {
        public RegisterGeneric1Param()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
        }
    }
}