using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    public abstract class RegisterFallback : LightInjectPatternBase
    {
        protected RegisterFallback(IStructuralSearchPattern pattern)
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

                if (invocationExpression.Arguments.Any() && invocationExpression.Arguments.Count >= 2)
                {
                    if (invocationExpression.Arguments[1].Value is ILambdaExpression lambdaExpression)
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
    }
}