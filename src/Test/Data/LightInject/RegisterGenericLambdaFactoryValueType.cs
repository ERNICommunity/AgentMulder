// Patterns: 1
// Matches: TakesIntAndInterface.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambdaFactoryValueType
    {
        public RegisterGenericLambdaFactoryValueType()
        {
            var container = new ServiceContainer();
            container.Register<int, IFoo>((factory, value) => new TakesIntAndInterface(value, factory.GetInstance<IFoo>()));
        }
    }
}