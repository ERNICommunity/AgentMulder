﻿using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using AgentMulder.ReSharper.Domain.Utils;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.Autofac.Patterns.FromAssemblies.BasedOn
{
    internal sealed class AsClosedTypesOf : MultipleMatchBasedOnPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$builder$.AsClosedTypesOf($argument$)",
                new ExpressionPlaceholder("builder"),
                new ArgumentPlaceholder("argument", 1, 1));

        public AsClosedTypesOf()
            : base(pattern)
        {
        }

        protected override IEnumerable<FilteredRegistrationBase> DoCreateRegistrations(ITreeNode registrationRootElement, IStructuralMatchResult match)
        {
            var argument = match.GetMatchedElement("argument") as ICSharpArgument;
            if (argument != null)
            {
                var typeOfExpression = argument.Value as ITypeofExpression;
                if (typeOfExpression != null)
                {
                    var argumentType = typeOfExpression.ArgumentType as IDeclaredType;
                    if (argumentType != null)
                    {
                        var typeElement = argumentType.GetTypeElement();
                        if (typeElement == null)
                        {
                            yield break;
                        }

                        yield return new ClosingTypesRegistration(registrationRootElement, typeElement);
                    }
                }
            }
        }

        private class ClosingTypesRegistration : FilteredRegistrationBase
        {
            public ClosingTypesRegistration(ITreeNode registrationRootElement, ITypeElement openGenericType)
                : base(registrationRootElement)
            {
                AddFilter(typeElement =>
                {
                    var @interface = openGenericType as IInterface;
                    if (!@interface.IsOpenGeneric())
                    {
                        return false;
                    }

                    return typeElement.ClosesOver(openGenericType);
                });
            }
        }
    }
}