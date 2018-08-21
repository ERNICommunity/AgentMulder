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
    public class Register : LightInjectPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.Register($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 0, 4));

        public Register()
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
                    else
                    {
                        foreach (var registration in FromArguments(invocationExpression))
                        {
                            yield return registration;
                        }
                    }
                }
            }
        }

        private IEnumerable<IComponentRegistration> FromGenericArguments(IInvocationExpression invocationExpression)
        {
            if (!invocationExpression.Arguments.Any())
            {
                foreach (var componentRegistration in GetRegistrationsFromTypeArguments(invocationExpression))
                {
                    yield return componentRegistration;
                }
            }
            else
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
                else
                {
                    foreach (var componentRegistration in GetRegistrationsFromTypeArguments(invocationExpression))
                    {
                        yield return componentRegistration;
                    }
                }
            }
        }

        private IEnumerable<IComponentRegistration> FromArguments(IInvocationExpression invocationExpression)
        {
            List<ITypeofExpression> arguments = invocationExpression.ArgumentList.Arguments
                .Where(argument =>
                {
                    var declaredType = argument.Value.Type() as IDeclaredType;
                    return declaredType != null && declaredType.IsType();
                }).Select(argument => argument.Value as ITypeofExpression)
                .ToList();

            var first = arguments.First().ArgumentType as IDeclaredType;
            var last = arguments.Last().ArgumentType as IDeclaredType;

            return CreateRegistration(invocationExpression, first, last);
        }

        private IEnumerable<IComponentRegistration> GetRegistrationsFromTypeArguments(IInvocationExpression invocationExpression)
        {
            var first = invocationExpression.TypeArguments.First() as IDeclaredType;
            var last = invocationExpression.TypeArguments.Last() as IDeclaredType;

            foreach (var registration in CreateRegistration(invocationExpression, first, last))
            {
                yield return registration;
            }
        }

        private IEnumerable<IComponentRegistration> CreateRegistration(IInvocationExpression invocationExpression, IDeclaredType first, IDeclaredType last)
        {
            if (first == null || last == null)
            {
                yield break;
            }

            ITypeElement fromType = first.GetTypeElement();
            ITypeElement toType = last.GetTypeElement();

            if (fromType != null && toType != null)
            {
                yield return fromType.Equals(toType)
                    ? new ComponentRegistration(invocationExpression, fromType)
                    : new ComponentRegistration(invocationExpression, fromType, toType);
            }
        }
    }
}