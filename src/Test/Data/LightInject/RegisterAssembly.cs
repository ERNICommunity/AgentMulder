// Patterns: 1
// Matches: Foo.cs, Bar.cs
// NotMatches: 

using System.Reflection;
using LightInject;

namespace TestApplication.LightInject
{
    public class RegisterAssembly
    {
        public RegisterAssembly()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
