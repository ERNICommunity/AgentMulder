// Patterns: 1
// Matches: TakesDelegate.cs
// NotMatches: Baz.cs

using LightInject;
using TestApplication.Types;

namespace TestApplication.LightInject
{
    public class RegisterGenericLambdaFactory
    {
        public RegisterGenericLambdaFactory()
        {
            var container = new ServiceContainer();
            container.Register<IFoo>(factory => new TakesDelegate(factory.GetInstance<IFoo>));
        }
    }
}