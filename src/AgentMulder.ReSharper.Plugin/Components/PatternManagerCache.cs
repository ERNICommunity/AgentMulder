﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers.impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentMulder.ReSharper.Plugin.Components
{
    [SolutionComponent]
    public class PatternManagerCache : IPatternManager, ICache
    {
        private readonly object lockObject = new object();
        private readonly JetHashSet<IPsiSourceFile> dirtyFiles = new JetHashSet<IPsiSourceFile>();
        private readonly PsiProjectFileTypeCoordinator projectFileTypeCoordinator;
        private readonly SolutionAnalyzer solutionAnalyzer;

        private readonly OneToListMap<IPsiSourceFile, RegistrationInfo> registrationsMap =
            new OneToListMap<IPsiSourceFile, RegistrationInfo>();

        public PatternManagerCache(SolutionAnalyzer solutionAnalyzer, PsiProjectFileTypeCoordinator projectFileTypeCoordinator)
        {
            this.projectFileTypeCoordinator = projectFileTypeCoordinator;
            this.solutionAnalyzer = solutionAnalyzer;
        }

        public IEnumerable<RegistrationInfo> GetRegistrationsForFile(IPsiSourceFile sourceFile)
        {
            if (!((ICache)this).UpToDate(sourceFile))
            {
                Merge(sourceFile, ProcessSourceFile(sourceFile));
            }

            lock (lockObject)
            {
                return registrationsMap.Values;
            }
        }

        object ICache.Build(IPsiSourceFile sourceFile, bool isStartup)
        {
            var registrationInfos = ProcessSourceFile(sourceFile).ToList();
            Merge(sourceFile, registrationInfos);
            return registrationInfos;
        }

        private IEnumerable<RegistrationInfo> ProcessSourceFile(IPsiSourceFile sourceFile)
        {
            return solutionAnalyzer.Analyze(sourceFile);
        }

        public void Dump(TextWriter writer, IPsiSourceFile sourceFile)
        {
            
        }

        public bool HasDirtyFiles => dirtyFiles.Any();

        void ICache.Save(IProgressIndicator progress, bool enablePersistence)
        {
            Save?.Invoke(this, EventArgs.Empty);
        }

        void ICache.MarkAsDirty(IPsiSourceFile sourceFile)
        {
            MarkAsDirty(sourceFile);
        }

        private void MarkAsDirty(IPsiSourceFile sourceFile)
        {
            lock (lockObject)
            {
                dirtyFiles.Add(sourceFile);
            }
        }

        void ICache.Merge(IPsiSourceFile sourceFile, object builtPart)
        {
            if (builtPart == null)
            {
                return;
            }

            Merge(sourceFile, (IEnumerable<RegistrationInfo>)builtPart);
        }

        void ICache.Drop(IPsiSourceFile sourceFile)
        {
            RemoveFileItems(sourceFile);
        }

        private void Merge(IPsiSourceFile sourceFile, IEnumerable<RegistrationInfo> items)
        {
            lock (lockObject)
            {
                registrationsMap.RemoveKey(sourceFile);
                registrationsMap.AddValueRange(sourceFile, items);

                dirtyFiles.Remove(sourceFile);
            }
        }

        object ICache.Load(IProgressIndicator progress, bool enablePersistence)
        {
            return null;
        }

        void ICache.MergeLoaded(object data)
        {
        }

        private void RemoveFileItems(IPsiSourceFile sourceFile)
        {
            lock (lockObject)
            {
                if (!registrationsMap.ContainsKey(sourceFile))
                {
                    return;
                }

                registrationsMap.RemoveKey(sourceFile);
            }
        }

        void ICache.OnPsiChange(ITreeNode elementContainingChanges, PsiChangedElementType type)
        {
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
            MarkAsDirty(sourceFile);
        }

        void ICache.SyncUpdate(bool underTransaction)
        {     
            lock (lockObject)
            {
                if (HasDirtyFiles)
                {
                    foreach (var psiSourceFile in dirtyFiles.ToList()) // ToList to prevent InvalidOperation while enumerating
                    {
                        ((ICache)this).Merge(psiSourceFile, ProcessSourceFile(psiSourceFile));
                    }

                    dirtyFiles.Clear();
                }
            }      
        }

        bool ICache.UpToDate(IPsiSourceFile sourceFile)
        {
            lock (lockObject)
            {
                if (dirtyFiles.Contains(sourceFile))
                {
                    return false;
                }

                if (!registrationsMap.ContainsKey(sourceFile))
                {
                    return false;
                }
            }

            return !ShouldBeProcessed(sourceFile);
        }

        private bool ShouldBeProcessed(IPsiSourceFile sourceFile)
        {
            if (!sourceFile.Properties.ShouldBuildPsi)
            {
                return false;
            }

            ProjectFileType languageType = sourceFile.LanguageType;
            return !languageType.IsNullOrUnknown() && projectFileTypeCoordinator.TryGetService(languageType) != null;
        }

        public IEnumerable<RegistrationInfo> GetAllRegistrations()
        {
            if (HasDirtyFiles)
            {
                ((ICache)this).SyncUpdate(false);
            }

            lock (lockObject)
            {
                return registrationsMap.Values;
            }
        }

        public event EventHandler Save;
    }
}