using System.Collections.Generic;
using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AspNetCore.Patterns
{
    // AddDbContext<TContext>() where TContext : DbContext 
    // AddDbContext<TContext>(options => ...) where TContext : DbContext 
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class AddDbContext : AspNetCorePatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.AddDbContext($arguments$)",
            new ExpressionPlaceholder("container", "Microsoft.Extensions.DependencyInjection.IServiceCollection"),
            new ArgumentPlaceholder("arguments", 0, 1));

        public AddDbContext() : base(pattern)
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

            if (invocationExpression.TypeArguments.Count == 1)
            {
                var serviceType = invocationExpression.TypeArguments[0] as IDeclaredType;

                var registration = CreateRegistration(invocationExpression, serviceType, serviceType);

                yield return registration;
            }
        }
    }
}
