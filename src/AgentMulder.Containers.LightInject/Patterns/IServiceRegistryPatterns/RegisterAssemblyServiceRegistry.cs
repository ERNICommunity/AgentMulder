using System;
using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.LightInject.Patterns.IServiceRegistryPatterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public sealed class RegisterAssemblyServiceRegistry : RegisterAssembly
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterAssembly($assembly$)",
                new ExpressionPlaceholder("container", "global::LightInject.IServiceRegistry", true),
                new ArgumentPlaceholder("assembly", 1, 1));

        public RegisterAssemblyServiceRegistry()
            : base(pattern)
        {
        }
    }
}