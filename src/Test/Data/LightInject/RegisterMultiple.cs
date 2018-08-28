// Patterns: 1
// Matches: Foo.cs,Baz.cs
// NotMatches: Bar.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterMultiple
    {
        public RegisterMultiple()
        {
            var container = new ServiceContainer();
            container.Register<IFoo>(provider =>
            {
                if (new object() == null)
                {
                    return new Foo();
                }
                else
                {
                    return new Baz();
                }
            });
        }
    }
}