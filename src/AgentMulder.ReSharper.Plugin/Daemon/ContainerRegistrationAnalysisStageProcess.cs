using System;
using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Plugin.Components;
using AgentMulder.ReSharper.Plugin.Highlighting;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.ReSharper.Plugin.Daemon
{
    public class ContainerRegistrationAnalysisStageProcess : IDaemonStageProcess
    {
        private readonly IContextBoundSettingsStore settingsStore;
        private readonly IPatternManager patternManager;
        private readonly TypeUsageManager usageManager;

        public ContainerRegistrationAnalysisStageProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, IPatternManager patternManager)
        {
            this.DaemonProcess = process;
            this.settingsStore = settingsStore;
            this.patternManager = patternManager;
            this.usageManager = new TypeUsageManager(process.GetStageProcess<CollectUsagesStageProcess>());
        }

        public void Execute(Action<DaemonStageResult> commiter)
        {
            var consumer = new DefaultHighlightingConsumer(this, settingsStore);

            foreach (IFile psiFile in EnumeratePsiFiles())
            {
                ProcessFile(psiFile, consumer);
            }

            commiter(new DaemonStageResult(consumer.Highlightings));
        }

        private void ProcessFile(IFile psiFile, DefaultHighlightingConsumer consumer)
        {
            foreach (var declaration in psiFile.ThisAndDescendants<ITypeDeclaration>())
            {
                if (declaration.DeclaredElement == null) // type is not (yet) declared
                {
                    return;
                }

                RegistrationInfo registrationInfo = patternManager.GetRegistrationsForFile(psiFile.GetSourceFile()).
                                                                   FirstOrDefault(c => c.Registration.IsSatisfiedBy(declaration.DeclaredElement));
                if (registrationInfo != null)
                {
                    this.usageManager.MarkTypeAsUsed(declaration); // this will set the type as used

                    consumer.AddHighlighting(new RegisteredByContainerHighlighting(registrationInfo),  declaration.GetNameDocumentRange());
                }
            }
        }


        private IEnumerable<IFile> EnumeratePsiFiles()
        {
            return DaemonProcess.SourceFile.EnumerateDominantPsiFiles();
        }

        public IDaemonProcess DaemonProcess { get; }
    }
}