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
    public class RegisterPropertyDependency : LightInjectPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterPropertyDependency($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 1, 1));

        public RegisterPropertyDependency()
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

                if (!invocationExpression.Arguments.Any())
                {
                    yield break;
                }

                var expression = invocationExpression.ArgumentList.Arguments.First().Expression;

                if (expression != null && expression is ILambdaExpression lambdaExpression)
                {
                    var implementationTypes = GetTypesFromLambda(lambdaExpression);

                    if (invocationExpression.TypeArguments.Count == 1)
                    {
                        var serviceType = invocationExpression.TypeArguments.First() as IDeclaredType;

                        if (serviceType == null)
                        {
                            yield break;
                        }

                        yield return new TypesBasedOnRegistration(implementationTypes.Select(_ => _.GetTypeElement()),
                            new ServiceRegistration(invocationExpression, serviceType.GetTypeElement()));
                    }
                    else
                    {
                        foreach (var implementationType in implementationTypes)
                        {
                            yield return new ComponentRegistration(registrationRootElement,
                                implementationType.GetTypeElement(), implementationType.GetTypeElement());
                        }
                    }
                }
            }
        }
    }
}
