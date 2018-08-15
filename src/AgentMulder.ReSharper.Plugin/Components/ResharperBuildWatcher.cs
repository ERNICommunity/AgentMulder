using System.Threading.Tasks;
using JetBrains.IDE.SolutionBuilders.Prototype.Services.Execution.Stubs;
using JetBrains.ProjectModel;

namespace AgentMulder.ReSharper.Plugin.Components
{
    [SolutionComponent]
    public sealed class ResharperBuildWatcher : BuildRunWrapperStub
    {
        private readonly ISolution solution;

        public ResharperBuildWatcher(ISolution solution)
        {
            this.solution = solution;
        }

        public override void AfterBuild()
        {
            base.AfterBuild();
            Task.Run(() =>
            {
                this.solution.GetComponent<IPatternManager>().Refresh();
                this.solution.GetComponent<IRegisteredTypeCollector>().Refresh();
            });
        }
    }
}