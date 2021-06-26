using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using JetBrains.ReSharper.Psi.VB.Util;

namespace AgentMulder.ReSharper.Plugin.Components
{
    /// <summary>
    /// A solution component for collecting and caching all DI-registered types.
    /// </summary>
    /// <remarks>This is constructed and injected automatically by the runtime.</remarks>
    [SolutionComponent]
    public sealed class RegisteredTypeCollector : IRegisteredTypeCollector, IPsiSourceFileCache
    {
        private readonly ConcurrentDictionary<IPsiSourceFile, object> dirtyFiles = new ConcurrentDictionary<IPsiSourceFile, object>();
        private readonly PsiProjectFileTypeCoordinator projectFileTypeCoordinator;
        private readonly IPatternManager patternManager;

        private readonly ConcurrentDictionary<IPsiSourceFile, List<MatchingType>> matchingTypes =
            new ConcurrentDictionary<IPsiSourceFile, List<MatchingType>>();

        public RegisteredTypeCollector(PsiProjectFileTypeCoordinator projectFileTypeCoordinator, IPatternManager patternManager)
        {
            this.projectFileTypeCoordinator = projectFileTypeCoordinator;
            this.patternManager = patternManager;
            this.patternManager.Save += (sender, args) => Refresh();
        }

        public bool HasDirtyFiles => dirtyFiles.Any();

        /// <summary>
        /// Gets all the currently registered types and the registrations that created them.
        /// </summary>
        /// <returns>A read-only collection of type declarations and registration information.</returns>
        public IEnumerable<Tuple<ITypeDeclaration, RegistrationInfo>>  GetRegisteredTypes()
        {
            ((IPsiSourceFileCache)this).SyncUpdate(false);

            // this cache stores declared types for each source file
            // when a source file changes, types in that file are recalculated
            // when asking for matching registrations, all known types are compared to existing registrations
            // types matching a registration are returned
            // we take the current snapshot of values and return
            var types = matchingTypes.Values.ToArray().SelectMany(_ => _).Select(
                _ => new Tuple<ITypeDeclaration, RegistrationInfo>(_.TypeDeclaration, _.MatchingRegistration));

            return types;
        }
        
        object IPsiSourceFileCache.Build(IPsiSourceFile sourceFile, bool isStartup)
        {
            if (sourceFile.Properties.IsGeneratedFile || sourceFile.Properties.IsNonUserFile)
            {
                return null;
            }

            // this will store only keys, no action is performed on the files
            // this is because to collect registered types, we first need the pattern manager cache to be completely populated
            // that will only happen later - see Refresh()
            var list = new List<MatchingType>();
            ((IPsiSourceFileCache)this).Merge(sourceFile, list);

            return list;
        }

        void IPsiSourceFileCache.MarkAsDirty(IPsiSourceFile sf)
        {
            dirtyFiles.AddOrUpdate(sf, _ => null, (file, current) => null);
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

        bool IPsiSourceFileCache.UpToDate(IPsiSourceFile sourceFile)
        {
            if (dirtyFiles.ContainsKey(sourceFile))
            {
                return false;
            }

            if (!matchingTypes.ContainsKey(sourceFile))
            {
                return false;
            }

            if (!sourceFile.Properties.ShouldBuildPsi)
            {
                return false;
            }

            var languageType = sourceFile.LanguageType;
            return !languageType.IsNullOrUnknown() && projectFileTypeCoordinator.TryGetService(languageType) != null;
        }

        void IPsiSourceFileCache.Merge(IPsiSourceFile sourceFile, object data)
        {
            // merges the provided data into the cache
            // this is usually called after Build
            if (data == null)
            {
                return;
            }

            var list = ((IEnumerable<MatchingType>)data).ToList();
            matchingTypes.AddOrUpdate(sourceFile, file => list, (file, current) => list);

            dirtyFiles.TryRemove(sourceFile, out _);
        }

        void IPsiSourceFileCache.Drop(IPsiSourceFile sourceFile)
        {
            // removes the specified file from the cache
            if (!matchingTypes.ContainsKey(sourceFile))
            {
                return;
            }

            matchingTypes.TryRemove(sourceFile, out _);
        }

        void IPsiSourceFileCache.OnPsiChange(ITreeNode elementContainingChanges, PsiChangedElementType type)
        {
            if (type == PsiChangedElementType.Whitespaces)
            {
                return;
            }

            var sourceFile = elementContainingChanges?.GetSourceFile();
            if (sourceFile == null)
            {
                return;
            }

            dirtyFiles.AddOrUpdate(sourceFile, _ => null, (file, current) => null);
        }

        void IPsiSourceFileCache.OnDocumentChange(IPsiSourceFile sourceFile, ProjectFileDocumentCopyChange change)
        {
            // marks the document as dirty
            ((IPsiSourceFileCache)this).MarkAsDirty(sourceFile);
        }

        void IPsiSourceFileCache.SyncUpdate(bool underTransaction)
        {
            if (underTransaction)
            {
                return;
            }

            if (!patternManager.GetAllRegistrations().Any())
            {
                dirtyFiles.Clear();
                return;
            }
        
            if (HasDirtyFiles)
            {
                foreach (var psiSourceFile in dirtyFiles.Keys.ToList()) // ToList to prevent InvalidOperation while enumerating
                {
                    ((IPsiSourceFileCache)this).Merge(psiSourceFile, CollectTypes(psiSourceFile));
                }

                dirtyFiles.Clear();
            }
        }

        void IPsiSourceFileCache.Dump(TextWriter writer, IPsiSourceFile sourceFile)
        {
            // this is just a debugging facility
        }

        private List<MatchingType> CollectTypes(IPsiSourceFile sourceFile)
        {
            // initializes the collection visitor and runs it against the specified file
            var visitor = new RegisteredTypeCollectorVisitor(patternManager.GetAllRegistrations().ToList());

            var searchDomain = SearchDomainFactory.Instance.CreateSearchDomain(sourceFile);

            searchDomain.Accept(visitor);

            return visitor.MatchingTypes;
        }

        /// <summary>
        /// Recomputes the collected types from scratch.
        /// </summary>
        public void Refresh()
        {
            foreach (var file in matchingTypes.Keys.ToList())
            {
                var list = CollectTypes(file);
                ((IPsiSourceFileCache)this).Merge(file, list);
            }
        }

        /// <summary>
        /// The visitor for collecting registered types.
        /// </summary>
        private class RegisteredTypeCollectorVisitor : SearchDomainVisitor
        {
            private readonly IList<RegistrationInfo> registrations;

            public override bool ProcessingIsFinished { get; } = false;

            public List<MatchingType> MatchingTypes { get; } = new List<MatchingType>();

            public RegisteredTypeCollectorVisitor(IList<RegistrationInfo> registrations)
            {
                this.registrations = registrations;
            }

            public override void VisitElement(ITreeNode element)
            {
                var typeDeclaration = element as ITypeDeclaration;

                if (typeDeclaration == null || !typeDeclaration.DeclaredElement.IsClass())
                {
                    // this element is not a class declaration - call recursively on children
                    foreach (var treeNode in element.Children())
                    {
                        VisitElement(treeNode);
                    }
                }
                else
                {
                    var matchingRegistration =
                        registrations.FirstOrDefault(
                            _ => _.Registration.IsSatisfiedBy(typeDeclaration.DeclaredElement));

                    if (matchingRegistration != null)
                    {
                        // this element is a class declaration
                        // we store it for later
                        MatchingTypes.Add(new MatchingType(typeDeclaration, matchingRegistration));
                    }
                    else
                    {
                        // this element is not a class declaration - call recursively on children
                        foreach (var treeNode in element.Children())
                        {
                            VisitElement(treeNode);
                        }
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

        private sealed class MatchingType
        {
            public MatchingType(ITypeDeclaration typeDeclaration, RegistrationInfo registration)
            {
                TypeDeclaration = typeDeclaration;
                MatchingRegistration = registration;
            }

            public ITypeDeclaration TypeDeclaration { get; }

            public RegistrationInfo MatchingRegistration { get; }
        }
    }
}