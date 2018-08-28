// Patterns: 1
// Matches: Foo.cs, Bar.cs
// NotMatches: 

using LightInject;

namespace TestApplication.LightInject
{
    public class RegisterAssemblyWithPattern
    {
        public RegisterAssemblyWithPattern()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly("Test*.dll");
        }
    }
}
