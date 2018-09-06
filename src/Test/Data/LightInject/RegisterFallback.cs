// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterFallback: ICompositionRoot
    {
        public RegisterFallback()
        {
            var container = new ServiceContainer();
            container.RegisterFallback((type, s) => true, request => new Foo());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterFallback((type, s) => true, request => new Bar());
        }
    }
}