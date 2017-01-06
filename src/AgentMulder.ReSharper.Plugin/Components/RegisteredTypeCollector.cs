using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers.impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using System.Linq;

namespace AgentMulder.ReSharper.Plugin.Components
{
    /// <summary>
    /// A solution component for collecting and caching all DI-registered types.
    /// </summary>
    /// <remarks>This is constructed and injected autoamtically by the runtime.</remarks>
    [SolutionComponent]
    public sealed class RegisteredTypeCollector : IRegisteredTypeCollector, ICache
    {
        private readonly object lockObject = new object();
        private readonly JetHashSet<IPsiSourceFile> dirtyFiles = new JetHashSet<IPsiSourceFile>();
        private readonly PsiProjectFileTypeCoordinator projectFileTypeCoordinator;
        private readonly IPatternManager patternManager;

        private readonly OneToListMap<IPsiSourceFile, Tuple<ITypeDeclaration, RegistrationInfo>> matchingTypes =
            new OneToListMap<IPsiSourceFile, Tuple<ITypeDeclaration, RegistrationInfo>>();

        public RegisteredTypeCollector(PsiProjectFileTypeCoordinator projectFileTypeCoordinator, IPatternManager patternManager)
        {
            this.projectFileTypeCoordinator = projectFileTypeCoordinator;
            this.patternManager = patternManager;
        }

        public bool HasDirtyFiles => dirtyFiles.Any();

        /// <summary>
        /// Gets all the currently registered types and the registrations that created them.
        /// </summary>
        /// <returns>A read-only collection of type declarations and registration information.</returns>
        public IReadOnlyCollection<Tuple<ITypeDeclaration, RegistrationInfo>> GetRegisteredTypes()
        {
            ((ICache)this).SyncUpdate(false);

            lock (lockObject)
            {
                return new ReadOnlyCollection<Tuple<ITypeDeclaration, RegistrationInfo>>(matchingTypes.Values.ToList());
            }
        }

        object ICache.Build(IPsiSourceFile sourceFile, bool isStartup)
        {
            // collect types from file
            return CollectTypes(sourceFile).ToList();
        }

        void ICache.MarkAsDirty(IPsiSourceFile sf)
        {
            lock (lockObject)
            {
                dirtyFiles.Add(sf);
            }
        }

        object ICache.Load(IProgressIndicator progress, bool enablePersistence)
        {
            // we do not persist this cache, no need to load anything
            return null;
        }

        void ICache.MergeLoaded(object data)
        {
            // we do not persist this cache, no need to merge loaded data
        }

        void ICache.Save(IProgressIndicator progress, bool enablePersistence)
        {
            // we do not persist this cache
        }

        bool ICache.UpToDate(IPsiSourceFile sourceFile)
        {
            lock (lockObject)
            {
                if (dirtyFiles.Contains(sourceFile))
                {
                    return false;
                }

                if (!matchingTypes.ContainsKey(sourceFile))
                {
                    return false;
                }
            }

            if (!sourceFile.Properties.ShouldBuildPsi)
            {
                return false;
            }

            var languageType = sourceFile.LanguageType;
            return !languageType.IsNullOrUnknown() && projectFileTypeCoordinator.TryGetService(languageType) != null;
        }

        void ICache.Merge(IPsiSourceFile sourceFile, object data)
        {
            // merges the provided data into the cache
            // this is usually called after Build
            if (data == null)
            {
                return;
            }

            lock (lockObject)
            {
                matchingTypes.RemoveKey(sourceFile);
                matchingTypes.AddValueRange(sourceFile, (IEnumerable<Tuple<ITypeDeclaration, RegistrationInfo>>)data);

                dirtyFiles.Remove(sourceFile);
            }
        }

        void ICache.Drop(IPsiSourceFile sourceFile)
        {
            // removes the specified file from the cache
            lock (lockObject)
            {
                if (!matchingTypes.ContainsKey(sourceFile))
                {
                    return;
                }

                matchingTypes.RemoveKey(sourceFile);
            }
        }

        void ICache.OnPsiChange(ITreeNode elementContainingChanges, PsiChangedElementType type)
        {
            //if (type == PsiChangedElementType.Whitespaces)
            //{
            //    return;
            //}

            var sourceFile = elementContainingChanges?.GetSourceFile();
            if (sourceFile == null)
            {
                return;
            }

            lock (lockObject)
            {
                dirtyFiles.Add(sourceFile);
            }
        }

        void ICache.OnDocumentChange(IPsiSourceFile sourceFile, ProjectFileDocumentCopyChange change)
        {
            // marks the document as dirty
            ((ICache)this).MarkAsDirty(sourceFile);
        }

        void ICache.SyncUpdate(bool underTransaction)
        {
            lock (lockObject)
            {
                if (HasDirtyFiles)
                {
                    foreach (var psiSourceFile in dirtyFiles)
                    {
                        ((ICache)this).Merge(psiSourceFile, CollectTypes(psiSourceFile));
                    }

                    dirtyFiles.Clear();
                }
            }
        }

        void ICache.Dump(TextWriter writer, IPsiSourceFile sourceFile)
        {
            // this is just a debugging facility
        }

        private IEnumerable<Tuple<ITypeDeclaration, RegistrationInfo>> CollectTypes(IPsiSourceFile sourceFile)
        {
            // initializes the collection visitor and runs it agains the specified file
            var visitor = new RegisteredTypeCollectorVisitor(patternManager.GetAllRegistrations().ToList());

            var searchDomain = SearchDomainFactory.Instance.CreateSearchDomain(sourceFile);

            searchDomain.Accept(visitor);

            return visitor.MatchingTypes;
        }

        /// <summary>
        /// The visitor for collecting registered types.
        /// </summary>
        private class RegisteredTypeCollectorVisitor : SearchDomainVisitor
        {
            private readonly IList<RegistrationInfo> registrations;

            public override bool ProcessingIsFinished { get; } = false;

            public List<Tuple<ITypeDeclaration, RegistrationInfo>> MatchingTypes { get; } = new List<Tuple<ITypeDeclaration, RegistrationInfo>>();

            public RegisteredTypeCollectorVisitor(IList<RegistrationInfo> registrations)
            {
                this.registrations = registrations;
            }

            public override void VisitElement(ITreeNode element)
            {
                var typeDeclaration = element as ITypeDeclaration;

                if (typeDeclaration == null)
                {
                    // this element is not a type declaration - call recursively on children
                    foreach (var treeNode in element.Children())
                    {
                        VisitElement(treeNode);
                    }
                }
                else
                {
                    // this element is a type declaration
                    // try to find a registration that matches this type
                    // if found, add it to the resulting collection
                    var matchingRegistration =
                        registrations.FirstOrDefault(_ => _.Registration.IsSatisfiedBy(typeDeclaration.DeclaredElement));
                    if (matchingRegistration != null)
                    {
                        MatchingTypes.Add(new Tuple<ITypeDeclaration, RegistrationInfo>(typeDeclaration, matchingRegistration));
                    }
                }
            }

            public override void VisitPsiSourceFile(IPsiSourceFile sourceFile)
            {
                base.VisitPsiSourceFile(sourceFile);
                foreach (var psiFile in sourceFile.GetPsiServices().Files.GetPsiFiles(sourceFile))
                {
                    VisitElement(psiFile);
                }
            }
        }
    }
}