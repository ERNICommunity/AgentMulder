// Patterns: 2
// Matches: TakesIntAndInterface.cs, TakesIntAndInterface1.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambdaFactoryValueType: ICompositionRoot
    {
        public RegisterGenericLambdaFactoryValueType()
        {
            var container = new ServiceContainer();
            container.Register<int, IFoo>((factory, value) => new TakesIntAndInterface(value, factory.GetInstance<IFoo>()));
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<int, IFoo>((factory, value) => new TakesIntAndInterface1(value, factory.GetInstance<IFoo>()));
        }
    }
}