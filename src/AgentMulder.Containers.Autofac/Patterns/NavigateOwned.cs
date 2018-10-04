using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentMulder.Containers.Autofac.Patterns
{
    [Export(typeof(INavigationProvider))]
    public class NavigateOwned : NavigationProvider
    {
        public override bool CheckOwned(ICSharpParameterDeclaration paramNode)
        {
            return string.Equals(paramNode?.Type?.GetScalarType()?.GetClrName().FullName,"Autofac.Features.OwnedInstances.Owned`1");
        }

    }
}
