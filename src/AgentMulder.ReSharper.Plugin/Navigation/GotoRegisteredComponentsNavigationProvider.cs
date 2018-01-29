using System;
using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Plugin.Components;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.TreeModels;
using JetBrains.DocumentModel.DataContext;
using JetBrains.IDE.TreeBrowser;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Navigation.ExecutionHosting;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;

namespace AgentMulder.ReSharper.Plugin.Navigation
{
    [ContextNavigationProvider]
    public class GotoRegisteredComponentsNavigationProvider : INavigateFromHereProvider
    {
        private const string ActionName = "Registered DI Components";

        public IEnumerable<ContextNavigation> CreateWorkflow(IDataContext dataContext)
        {
            var solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION);
            if (solution == null)
            {
                throw new InvalidOperationException("Unable to get the solution");
            }

            var patternManager = solution.GetComponent<IPatternManager>();
            var typeCollector = solution.GetComponent<IRegisteredTypeCollector>();

            var navigationExecutionHost = DefaultNavigationExecutionHost.GetInstance(solution);
            var execution = GetNavigationAction(dataContext, navigationExecutionHost, patternManager, solution, typeCollector);

            if (execution != null)
            {
                yield return
                    new ContextNavigation(ActionName, "GotoRegistered", NavigationActionGroup.Blessed,
                        execution, "GotoRegisteredShort");
            }
        }

        private static Action GetNavigationAction(IDataContext dataContext,
            INavigationExecutionHost navigationExecutionHost, [NotNull] IPatternManager patternManager,
            [NotNull] ISolution solution, [NotNull] IRegisteredTypeCollector typeCollector)
        {
            var invokedNode = dataContext.GetSelectedTreeNode<IExpression>();

            var document = dataContext.GetData(DocumentModelDataConstants.DOCUMENT);

            var psiSourceFile = document?.GetPsiSourceFile(solution);
            if (psiSourceFile == null)
            {
                return null;
            }

            var registration =
                patternManager.GetRegistrationsForFile(psiSourceFile)
                    .FirstOrDefault(r => r.Registration.RegistrationElement.Children().Contains(invokedNode));
            if (registration == null)
            {
                return null;
            }

            var registeredTypes = typeCollector.GetRegisteredTypes();

            return () =>
            {
                var occurences =
                    registeredTypes.Select(_ => _.Item1)
                        .Where(_ => _.DeclaredElement != null)
                        .Where(_ => registration.Registration.IsSatisfiedBy(_.DeclaredElement))
                        .Select(_ => new DeclaredElementOccurrence(_.DeclaredElement))
                        .Cast<IOccurrence>()
                        .ToList();

                navigationExecutionHost.ShowContextPopupMenu(dataContext, occurences,
                    DescriptorBuilder(solution, occurences),
                    new OccurrencePresentationOptions(IconDisplayStyle.OccurrenceEntityType, containerStyle: ContainerDisplayStyle.Namespace), true, ActionName);
            };
        }

        private static Func<IOccurrenceBrowserDescriptor> DescriptorBuilder(ISolution solution, IEnumerable<IOccurrence> occurences)
        {
            return () => new RegisteredComponentDescriptor(solution, occurences);
        }

        private class RegisteredComponentDescriptor : OccurrenceBrowserDescriptor
        {
            public RegisteredComponentDescriptor([NotNull] ISolution solution, [NotNull] IEnumerable<IOccurrence> occurrences)
                : base(solution)
            {
                using (ReadLockCookie.Create())
                {
                    this.SetResults(occurrences.ToList());
                }

                var treeSimpleModel = new TreeSimpleModel();
                Model = treeSimpleModel;

                foreach (var occurrence in occurrences)
                {
                    treeSimpleModel.Insert(null, occurrence);
                }

                Title.Value = ActionName;
            }

            public override TreeModel Model { get; }

            protected override void SetResults(ICollection<IOccurrence> items, IProgressIndicator indicator = null, bool mergeItems = true)
            {
                base.SetResults(items, indicator, mergeItems);
                this.RequestUpdate(UpdateKind.Display, true);
            }
        }
    }
}
