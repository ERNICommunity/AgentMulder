using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;

namespace AgentMulder.Containers.LightInject.Patterns
{
    public abstract class RegisterInstance : RegisterWithService
    {
        protected RegisterInstance(IStructuralSearchPattern pattern)
            : base(pattern)
        {
        }
    }
}