using System.Collections.Generic;
using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.AspNetCore.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class AddMvc : RegistrationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.AddMvc($arguments$)",
                new ExpressionPlaceholder("container", "Microsoft.Extensions.DependencyInjection.IServiceCollection"),
                new ArgumentPlaceholder("arguments", 0, 1));

        public AddMvc() : base(pattern)
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            var match = Match(registrationRootElement);

            if (match.Matched)
            {
                yield return new MvcControllerRegistration(registrationRootElement);
            }
        }

        /// <summary>
        /// Custom registration, limiting the valid types to descendants of the Microsoft.AspNetCore.Mvc.Controller class.
        /// </summary>
        private class MvcControllerRegistration : FilteredRegistrationBase
        {
            public MvcControllerRegistration(ITreeNode registrationRootElement)
                : base(registrationRootElement)
            {
                AddFilter(typeElement =>
                {
                    var mvcControllerType =
                        TypeFactory.CreateTypeByCLRName("Microsoft.AspNetCore.Mvc.Controller",
                            typeElement.Module).GetTypeElement();

                    return typeElement.IsDescendantOf(mvcControllerType);
                });
            }
        }
    }
}