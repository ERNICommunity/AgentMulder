using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterConstructorDependency : LightInjectPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterConstructorDependency($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 1, 1));

        public RegisterConstructorDependency()
            : base(pattern)
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IStructuralMatchResult match = Match(registrationRootElement);

            if (match.Matched)
            {
                var invocationExpression = match.MatchedElement as IInvocationExpression;
                if (invocationExpression == null)
                {
                    yield break;
                }

                if (invocationExpression.TypeArguments.Any())
                {
                    foreach (var registration in FromGenericArguments(invocationExpression))
                    {
                        yield return registration;
                    }
                }
                else if (invocationExpression.Arguments.Any())
                {
                    if (invocationExpression.Arguments.First().Value is ILambdaExpression lambdaExpression)
                    {
                        var implementationTypes = GetTypesFromLambda(lambdaExpression);
                        foreach (var implementationType in implementationTypes)
                        {
                            var type = implementationType.GetTypeElement();

                            if (type != null)
                            {
                                yield return new ComponentRegistration(invocationExpression,
                                    implementationType.GetTypeElement());
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<IComponentRegistration> FromGenericArguments(IInvocationExpression invocationExpression)
        {
                if (invocationExpression.Arguments.First().Value is ILambdaExpression lambdaExpression)
                {
                    var implementationTypes = GetTypesFromLambda(lambdaExpression);

                    var serviceType = invocationExpression.TypeArguments.Last() as IDeclaredType;

                    if (serviceType != null)
                    {
                        yield return new TypesBasedOnRegistration(implementationTypes.Select(_ => _.GetTypeElement()),
                            new ServiceRegistration(invocationExpression, serviceType.GetTypeElement()));
                    }
                }
        }
    }
}