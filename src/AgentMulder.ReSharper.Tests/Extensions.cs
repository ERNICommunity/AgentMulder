using System;
using System.Linq;
using AgentMulder.ReSharper.Domain.Containers;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.ReSharper.Tests
{
    internal static class Extensions
    {
        public static void ProcessChildren<T>(this ITreeNode element, [InstantHandle] Action<T> handler) where T : class, ITreeNode
        {
            foreach (T treeNode in element.ThisAndDescendants<T>())
            {
                if (treeNode != null)
                    handler(treeNode);
            }
        }

        /// <summary>
        /// Removes the type information from the search patterns.
        /// This is a workaround for type resolution issues in R# 2016.3.
        /// </summary>
        /// <param name="containerInfo">Container to strip type information from.</param>
        internal static void RemovePlaceholderTypes(this IContainerInfo containerInfo)
        {
            foreach (var regPattern in containerInfo.RegistrationPatterns)
            {
                foreach (var placeholder in regPattern.Pattern.Placeholders.Values.OfType<ExpressionPlaceholder>())
                {
                    placeholder.ExpressionType = string.Empty;
                    placeholder.ExactType = false;
                }
            }
        }
    }
}