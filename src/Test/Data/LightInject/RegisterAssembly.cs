// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: 

using System.Reflection;
using LightInject;

namespace TestApplication.LightInject
{
    public class RegisterAssembly: ICompositionRoot
    {
        public RegisterAssembly()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
