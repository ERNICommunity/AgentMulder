﻿using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.Autofac.Patterns.FromAssemblies.BasedOn
{
    internal sealed class AsGeneric : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$builder$.As<$service$>()",
                new ExpressionPlaceholder("builder"),
                new TypePlaceholder("service"));

        public AsGeneric()
            : base(pattern)
        {
        }

        protected override IEnumerable<FilteredRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var matchedType = match.GetMatchedType("service") as IDeclaredType;
            if (matchedType != null)
            {
                ITypeElement typeElement = matchedType.GetTypeElement();
                if (typeElement != null)
                {
                    yield return new ServiceRegistration(registrationRootElement, typeElement);
                }
            }
        }
    }
}