using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JetBrains.ReSharper.Psi;
using System.ComponentModel.Composition.Primitives;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentMulder.ReSharper.Domain.Patterns
{
    [InheritedExport(typeof(INavigationProvider))]
    public abstract class NavigationProvider : INavigationProvider
    {
        public abstract bool CheckOwned(ICSharpParameterDeclaration parameterType);
    }

}
