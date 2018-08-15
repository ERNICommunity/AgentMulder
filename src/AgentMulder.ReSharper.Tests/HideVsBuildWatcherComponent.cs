using AgentMulder.ReSharper.Plugin.Components;
using JetBrains.Application.Components;
using JetBrains.ProjectModel;

namespace AgentMulder.ReSharper.Tests
{
    [SolutionComponent]
    public class HideVsBuildWatcherComponent : IHideImplementation<VsBuildWatcher>
    {
        // this component gets loaded only at test time
        // it hides the VsBuildWatcher from the test host
        // not loading this will prevent attempting to inject DTE, which does not exist at test time
    }
}