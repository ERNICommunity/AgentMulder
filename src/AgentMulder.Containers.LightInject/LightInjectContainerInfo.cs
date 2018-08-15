using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Domain.Elements.Modules;
using AgentMulder.ReSharper.Domain.Elements.Modules.Impl;

namespace AgentMulder.Containers.LightInject
{
    [Export(typeof(IContainerInfo))]
    public class LightInjectContainerInfo : ContainerInfoBase
    {
        public override string ContainerDisplayName => "LightInject";

        public override IEnumerable<string> ContainerQualifiedNames
        {
            get { yield return "LightInject"; }
        }

        public LightInjectContainerInfo()
        {
            ModuleExtractor.AddExtractor(new CallingAssemblyExtractor("LightInject.Module", "ThisAssembly"));
        }

        protected override ComposablePartCatalog GetComponentCatalog()
        {
            return new AssemblyCatalog(Assembly.GetExecutingAssembly());
        }
    }
}