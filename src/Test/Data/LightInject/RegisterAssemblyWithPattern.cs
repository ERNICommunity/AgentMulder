// Patterns: 2
// Matches: Foo.cs, Bar.cs
// NotMatches: 

using LightInject;

namespace TestApplication.LightInject
{
    public class RegisterAssemblyWithPattern: ICompositionRoot
    {
        public RegisterAssemblyWithPattern()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly("Test*.dll");
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterAssembly("Test*.dll");
        }
    }
}
