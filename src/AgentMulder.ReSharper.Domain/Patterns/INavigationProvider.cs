using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentMulder.ReSharper.Domain.Patterns
{
    public interface INavigationProvider
    {
         bool CheckOwned(ICSharpParameterDeclaration parameterType);
    }
}
