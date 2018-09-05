using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using AgentMulder.ReSharper.Domain.Patterns;
using Autofac.Features.OwnedInstances;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Psi.VB.Types;

namespace AgentMulder.Containers.Autofac.Patterns
{
    [Export(typeof(INavigationProvider))]
    public class NavigateOwned : NavigationProvider
    {
        public override bool CheckOwned(ICSharpParameterDeclaration paramNode)
        {
            var result = paramNode?.Type?.GetScalarType()?.GetClrName().ShortName.ToLower() == "owned";
            return result;
        }

    }
}
