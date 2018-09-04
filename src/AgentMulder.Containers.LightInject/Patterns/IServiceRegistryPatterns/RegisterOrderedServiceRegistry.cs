using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.LightInject.Patterns.IServiceRegistryPatterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public class RegisterOrderedServiceRegistry : RegisterOrdered
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterOrdered($arguments$)",
                new ExpressionPlaceholder("container", "global::LightInject.IServiceRegistry", true),
                new ArgumentPlaceholder("arguments", 3, 4));

        public RegisterOrderedServiceRegistry()
            : base(pattern)
        {
        }
    }
}