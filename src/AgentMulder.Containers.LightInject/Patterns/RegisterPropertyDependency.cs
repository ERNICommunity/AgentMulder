using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    public abstract class RegisterPropertyDependency : LightInjectPatternBase
    {
        protected RegisterPropertyDependency(IStructuralSearchPattern pattern)
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
                    var implementationTypes = GetTypesFromLambda(lambdaExpression).ToList();

                    if (invocationExpression.TypeArguments.Count == 1)
                    {
                        var serviceType = invocationExpression.TypeArguments.First() as IDeclaredType;

                        if (serviceType == null || !implementationTypes.Any())
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
