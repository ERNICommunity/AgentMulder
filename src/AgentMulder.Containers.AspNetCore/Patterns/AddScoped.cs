using System.ComponentModel.Composition;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.AspNetCore.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class AddScoped : AspNetCorePatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.AddScoped($arguments$)",
                new ExpressionPlaceholder("container", "Microsoft.Extensions.DependencyInjection.IServiceCollection"),
                new ArgumentPlaceholder("arguments", 0, 2));

        public AddScoped() : base(pattern)
        {
        }
    }
}