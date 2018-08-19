using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using AgentMulder.ReSharper.Domain.Utils;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterOthers : RegisterWithService
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterOrdered($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 3, 4));

        public RegisterOthers()
            : base(pattern)
        {
        }

        protected override IEnumerable<IComponentRegistration> FromArguments(IInvocationExpression invocationExpression)
        {
            ICSharpArgument arg1 = invocationExpression.Arguments.First();

            ITypeElement typeElement = arg1.With(f => f.Value as ITypeofExpression)
                                           .With(f => f.ArgumentType as IDeclaredType)
                                           .With(f => f.GetTypeElement());
            if (typeElement == null)
            {
                yield break;
            }

            ICSharpArgument arg2 = invocationExpression.Arguments[1];
            IEnumerable<ITypeElement> concreteTypes = arg2.With(f => f.Value)
                                                          .With(f => f.GetRegisteredTypes()).ToList();

            if (concreteTypes.Any())
            {
                yield return CreateRegistrations(invocationExpression, typeElement, concreteTypes);
            }
        }

        private static IComponentRegistration CreateRegistrations(IInvocationExpression invocationExpression, ITypeElement serviceType, IEnumerable<ITypeElement> concreteTypes)
        {
            return new TypesBasedOnRegistration(concreteTypes, new ServiceRegistration(invocationExpression, serviceType));
        }
    }
}