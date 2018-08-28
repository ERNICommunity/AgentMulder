// Patterns: 1
// Matches: Foo.cs,Baz.cs
// NotMatches: Bar.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterOrdered
    {
        public RegisterOrdered()
        {
            var container = new ServiceContainer();
            container.RegisterOrdered(typeof(IFoo), new[] { typeof(Foo), typeof(Baz) }, type => new PerContainerLifetime());
        }
    }
}