using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AspNetCore
{
    /// <summary>
    /// Base class for all ASP.NET Core registration patterns.
    /// </summary>
    public abstract class AspNetCorePatternBase : RegistrationPatternBase
    {
        /// <param name="pattern">The SSR pattern.</param>
        protected AspNetCorePatternBase(IStructuralSearchPattern pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Gets the component registrations present in the given syntactic element.
        /// </summary>
        /// <param name="registrationRootElement">The syntactic element to scan for registrations.</param>
        /// <returns>A collection of component registrations.</returns>
        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            // ReSharper does not currently match generic and non-generic overloads separately, meaning that Register<T> and Register(typeof(T))
            // will be both matched with a single pattern Register($arguments$).
            // Therefire I am using this pattern to look for both generic and non-generic (with typeof) overloads of the pattern

            var match = Match(registrationRootElement);

            if (!match.Matched)
            {
                yield break;
            }

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
            else
            {
                foreach (var registration in FromArguments(invocationExpression))
                {
                    yield return registration;
                }
            }
        }

        /// <summary>
        /// Creates component registrations from the invocation expression's call arguments.
        /// </summary>
        /// <param name="invocationExpression">The invocation expression to read registrations from.</param>
        /// <returns>A collection of component registrations.</returns>
        private static IEnumerable<IComponentRegistration> FromArguments(IInvocationExpression invocationExpression)
        {
            if (invocationExpression.Arguments.Count == 1)
            {
                var registeredTypes = GetArgumentTypes(invocationExpression);

                foreach (var registeredType in registeredTypes)
                {
                    var registration = CreateRegistration(invocationExpression, registeredType, registeredType);
                    if (registration != null)
                    {
                        yield return registration;
                    }
                }
            }

            if (invocationExpression.Arguments.Count == 2)
            {
                var serviceTypes = GetArgumentTypes(invocationExpression, argumentIndex: 0);
                var implementationTypes = GetArgumentTypes(invocationExpression, argumentIndex: 1);

                foreach (var serviceType in serviceTypes)
                {
                    foreach (var implementationType in implementationTypes)
                    {
                        var registration = CreateRegistration(invocationExpression, serviceType, implementationType);
                        if (registration != null)
                        {
                            yield return registration;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates component registrations from the invocation expression's type arguments (generics).
        /// </summary>
        /// <param name="invocationExpression">The invocation expression to read registrations from.</param>
        /// <returns>A collection of component registrations.</returns>
        private static IEnumerable<IComponentRegistration> FromGenericArguments(IInvocationExpression invocationExpression)
        {
            if (invocationExpression.TypeArguments.Count == 1)
            {
                var serviceType = invocationExpression.TypeArguments[0] as IDeclaredType;

                if (invocationExpression.Arguments.Count == 0)
                {
                    var registration = CreateRegistration(invocationExpression, serviceType, serviceType);
                    if (registration != null)
                    {
                        yield return registration;
                    }
                }
                else if (invocationExpression.Arguments.Count == 1)
                {
                    var implementationTypes = GetArgumentTypes(invocationExpression);
                    foreach (var implementationType in implementationTypes)
                    {
                        var registration = CreateRegistration(invocationExpression, serviceType, implementationType);
                        if (registration != null)
                        {
                            yield return registration;
                        }
                    }
                }
            }

            if (invocationExpression.TypeArguments.Count == 2)
            {
                var serviceType = invocationExpression.TypeArguments[0] as IDeclaredType;
                var implementationType = invocationExpression.TypeArguments[1] as IDeclaredType;

                var registration = CreateRegistration(invocationExpression, serviceType, implementationType);
                if (registration != null)
                {
                    yield return registration; 
                }
            }
        }

        /// <summary>
        /// Gets the registration types present in the specified invocation expression.
        /// </summary>
        /// <param name="invocationExpression">The expression to scan for registration types.</param>
        /// <param name="argumentIndex">The index of the argument to scan for types.</param>
        /// <returns>Collection of registration types.</returns>
        private static IEnumerable<IDeclaredType> GetArgumentTypes(IInvocationExpression invocationExpression, int argumentIndex = 0)
        {
            // match typeof() expressions
            var typeOfExpression = invocationExpression.ArgumentList.Arguments[argumentIndex].Expression as ITypeofExpression;
            if (typeOfExpression != null)
            {
                var typeElement = (IDeclaredType)typeOfExpression.ArgumentType;

                yield return typeElement;
            }

            // new statement
            var objectCreationExpression = invocationExpression.ArgumentList.Arguments[argumentIndex].Expression as IObjectCreationExpression;
            if (objectCreationExpression != null)
            {
                yield return objectCreationExpression.GetExpressionType() as IDeclaredType;
            }

            // match lambda expressions
            var lambdaExpression = invocationExpression.ArgumentList.Arguments[argumentIndex].Expression as ILambdaExpression;
            if (lambdaExpression != null)
            {
                IDeclaredType declaredType = null;
                if (lambdaExpression.BodyBlock != null)
                {
                    // lambda with statement body
                    var returnTypes =
                        lambdaExpression.BodyBlock.Descendants<IReturnStatement>()
                            .ToEnumerable()
                            .Select(_ => _.Value.GetExpressionType() as IDeclaredType)
                            .Where(_ => _ != null && _.Classify == TypeClassification.REFERENCE_TYPE);

                    foreach (var returnType in returnTypes)
                    {
                        yield return returnType;
                    }
                }
                else if (lambdaExpression.BodyExpression != null)
                {
                    // lambda with expression body
                    declaredType = lambdaExpression.BodyExpression.GetExpressionType() as IDeclaredType;
                }

                if (declaredType != null && declaredType.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    yield return declaredType;
                }

                yield break;
            }

            // variable
            var referenceExpression = invocationExpression.ArgumentList.Arguments[argumentIndex].Expression as IReferenceExpression;
            if (referenceExpression != null)
            {
                yield return referenceExpression.GetExpressionType() as IDeclaredType;
            }

            // invocation expression
            var invocation = invocationExpression.ArgumentList.Arguments[argumentIndex].Expression as IInvocationExpression;
            if (invocation != null)
            {
                yield return invocation.GetExpressionType() as IDeclaredType;
            }
        }

        /// <summary>
        /// Creates a new component registration descriptor.
        /// </summary>
        /// <param name="invocationExpression">The source invocation expression.</param>
        /// <param name="serviceType">The type of the service being registered.</param>
        /// <param name="implementationType">The implementation type for the service.</param>
        /// <returns>A new component registration.</returns>
        protected static IComponentRegistration CreateRegistration(ITreeNode invocationExpression, IDeclaredType serviceType, IDeclaredType implementationType)
        {
            if (serviceType == null || implementationType == null)
            {
                return null;
            }

            var fromType = serviceType.GetTypeElement();
            var toType = implementationType.GetTypeElement();

            if (fromType != null && toType != null)
            {
                return fromType.Equals(toType)
                    ? new ComponentRegistration(invocationExpression, fromType)
                    : new ComponentRegistration(invocationExpression, fromType, toType);
            }

            return null;
        }
    }
}