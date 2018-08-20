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
using JetBrains.ReSharper.Psi.Util;

namespace AgentMulder.Containers.LightInject.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class Register : RegisterWithService
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.Register($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 0, 4));

        public Register()
            : base(pattern)
        {
        }

        protected override IEnumerable<IComponentRegistration> FromGenericArguments(IInvocationExpression invocationExpression)
        {
            if (invocationExpression.TypeArguments.Count == 1)
            {
                if (invocationExpression.Arguments.Any())
                {
                    var service = invocationExpression.TypeArguments.First() as IDeclaredType;

                    foreach (var componentRegistration in GetRegistrationsFromLambda(invocationExpression, service))
                    {
                        yield return componentRegistration;
                    }
                }

                if (invocationExpression.TypeArguments.First() is IDeclaredType implementation)
                {
                    ITypeElement typeElement = implementation.GetTypeElement();

                    if (typeElement != null)
                    {
                       yield return new ComponentRegistration(invocationExpression, typeElement);
                    }
                }
            }
            else
            {
                var first = invocationExpression.TypeArguments.First() as IDeclaredType;
                var last = invocationExpression.TypeArguments.Last() as IDeclaredType;

                if (first == null || last == null)
                {
                    yield break;
                }

                if (first.IsClassType() || first.IsInterfaceType())
                {
                    var service = first.GetTypeElement();
                    var @class = last.GetTypeElement();

                    yield return new ComponentRegistration(invocationExpression, service, @class);
                }
                else
                {
                    foreach (var componentRegistration in GetRegistrationsFromLambda(invocationExpression, last))
                    {
                        yield return componentRegistration;
                    }
                }
            }
        }

        private IEnumerable<IComponentRegistration> GetRegistrationsFromLambda(IInvocationExpression invocationExpression, IDeclaredType serviceType)
        {
            if (!invocationExpression.ArgumentList.Arguments.Any())
            {
                yield break;
            }

            if (invocationExpression.ArgumentList.Arguments.First().Value is ILambdaExpression lambdaExpression)
            {
                var objectCreationExpression = lambdaExpression.BodyExpression as IObjectCreationExpression;

                if (objectCreationExpression?.TypeReference != null)
                {
                    IResolveResult resolveResult = objectCreationExpression.TypeReference.Resolve().Result;

                    var service = serviceType.GetTypeElement();
                    var @class = resolveResult.DeclaredElement as IClass;

                    if (@class != null)
                    {
                        yield return new ComponentRegistration(invocationExpression, service, @class);
                    }
                }
            }
        }
    }
}