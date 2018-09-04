using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.LightInject.Patterns.ServiceContainerPatterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterServiceRegistry : Register
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.Register($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.IServiceRegistry", true),
                new ArgumentPlaceholder("arguments", 0, 4));

        public RegisterServiceRegistry()
            : base(pattern)
        {
        }
    }
}