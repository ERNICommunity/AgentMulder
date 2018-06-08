using System;
using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.Autofac.Patterns.FromAssemblies.BasedOn
{
    internal sealed class AssignableToNonGeneric : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$builder$.AssignableTo($argument$)",
                new ExpressionPlaceholder("builder"),
                new ArgumentPlaceholder("argument"));

        public AssignableToNonGeneric()
            : base(pattern)
        {
        }

        protected override IEnumerable<FilteredRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var argument = match.GetMatchedElement("argument") as ICSharpArgument;
            if (argument == null)
            {
                yield break;
            }

            var typeofExpression = argument.Value as ITypeofExpression;
            if (typeofExpression != null)
            {
                var declaredType = typeofExpression.ArgumentType as IDeclaredType;
                if (declaredType != null)
                {
                    ITypeElement typeElement = declaredType.GetTypeElement();
                    if (typeElement != null)
                    {
                        // todo possible bug: same as in the generic variant. Currently works the same as As<T>.
                        yield return new ServiceRegistration(registrationRootElement, typeElement);
                    }
                }
            }
        }
    }
}