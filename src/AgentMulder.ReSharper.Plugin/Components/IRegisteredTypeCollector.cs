using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.ReSharper.Plugin.Components
{
    /// <summary>
    /// A solution component for collecting and caching all DI-registered types.
    /// </summary>
    public interface IRegisteredTypeCollector
    {
        /// <summary>
        /// Gets all the currently registered types and the registrations that created them.
        /// </summary>
        /// <returns>A read-only collection of type declarations and registration information.</returns>
        IEnumerable<Tuple<ITypeDeclaration, RegistrationInfo>> GetRegisteredTypes();

        void Refresh();
    }
}