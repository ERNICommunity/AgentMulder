// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterFallback
    {
        public RegisterFallback()
        {
            var container = new ServiceContainer();
            container.RegisterFallback((type, s) => true, request => new Foo());
        }
    }
}