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
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterPropertyDependency : RegistrationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterPropertyDependency($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", -1, -1));


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

                var implementationType = GetImplementationType(invocationExpression);
                if (implementationType == null)
                {
                    yield break;
                }

                if (invocationExpression.TypeArguments.Count == 1)
                {
                    var serviceType = invocationExpression.TypeArguments.First() as IDeclaredType;

                    if (serviceType == null)
                    {
                        yield break;
                    }
                    yield return new ComponentRegistration(registrationRootElement, serviceType.GetTypeElement(), implementationType);
                }

                yield return new ComponentRegistration(registrationRootElement, implementationType, implementationType);
            }
        }

        private IClass GetImplementationType(IInvocationExpression invocationExpression)
        {
            if (!invocationExpression.ArgumentList.Arguments.Any())
            {
                return null;
            }

            if (invocationExpression.ArgumentList.Arguments.First().Value is ILambdaExpression lambdaExpression)
            {
                var objectCreationExpression = lambdaExpression.BodyExpression as IObjectCreationExpression;

                if (objectCreationExpression?.TypeReference != null)
                {
                    IResolveResult resolveResult = objectCreationExpression.TypeReference.Resolve().Result;

                    return resolveResult.DeclaredElement as IClass;
                }
            }

            return null;
        }
    }
}
