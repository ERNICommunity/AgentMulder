// Patterns: 1
// Matches: Foo.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambda
    {
        public RegisterGenericLambda()
        {
            var container = new ServiceContainer();
            container.Register<IFoo>(x => new Foo());
        }
    }
}