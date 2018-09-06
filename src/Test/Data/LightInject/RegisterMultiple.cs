// Patterns: 2
// Matches: Foo.cs,Baz.cs,FooBar.cs,Bar.cs
// NotMatches: 

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterMultiple: ICompositionRoot
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

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IBar>(provider =>
            {
                if (new object() == null)
                {
                    return new FooBar();
                }
                else
                {
                    return new Bar();
                }
            });
        }
    }
}