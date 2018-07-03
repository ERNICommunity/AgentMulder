using System.Threading.Tasks;
using EnvDTE;
using JetBrains.ProjectModel;

namespace AgentMulder.ReSharper.Plugin.Components
{
    [SolutionComponent]
    public sealed class VsBuildWatcher
    {
        private readonly ISolution solution;
        private BuildEvents events;

        public VsBuildWatcher(DTE dte, ISolution solution)
        {
            this.solution = solution;
            this.events = dte.Events.BuildEvents;
            
            this.events.OnBuildDone -= this.BuildDone;
            this.events.OnBuildDone += this.BuildDone;
        }

        private void BuildDone(vsBuildScope scope, vsBuildAction action)
        {
            Task.Run(() =>
            {
                this.solution.GetComponent<IPatternManager>().Refresh();
                this.solution.GetComponent<IRegisteredTypeCollector>().Refresh();
            });
        }
    }
}