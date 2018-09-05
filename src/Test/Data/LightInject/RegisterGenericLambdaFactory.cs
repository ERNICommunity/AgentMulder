// Patterns: 2
// Matches: TakesDelegate.cs, TakesDelegate1.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambdaFactory: ICompositionRoot
    {
        public RegisterGenericLambdaFactory()
        {
            var container = new ServiceContainer();
            container.Register<IFoo>(factory => new TakesDelegate(factory.GetInstance<IFoo>));
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IFoo>(factory => new TakesDelegate1(factory.GetInstance<IFoo>));
        }
    }
}