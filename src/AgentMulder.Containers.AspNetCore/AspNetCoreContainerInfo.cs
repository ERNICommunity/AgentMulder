using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using AgentMulder.ReSharper.Domain.Containers;

namespace AgentMulder.Containers.AspNetCore
{
    [Export(typeof(IContainerInfo))]
    public class AspNetCoreContainerInfo : ContainerInfoBase
    {
        public override string ContainerDisplayName => "ASP.NET Core DI";

        public override IEnumerable<string> ContainerQualifiedNames
        {
            get { yield return "Microsoft.Extensions.DependencyInjection"; }
        }

        protected override ComposablePartCatalog GetComponentCatalog()
        {
            return new AssemblyCatalog(Assembly.GetExecutingAssembly());
        }
    }
}
