using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.LightInject.Patterns.ServiceContainerPatterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterInstanceService : RegisterInstance
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterInstance($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("arguments", 1, 3));

        public RegisterInstanceService()
            : base(pattern)
        {
        }
    }
}