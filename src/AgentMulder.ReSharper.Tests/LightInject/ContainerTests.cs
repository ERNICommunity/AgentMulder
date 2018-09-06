using AgentMulder.Containers.LightInject;

namespace AgentMulder.ReSharper.Tests.LightInject
{
    [TestWithNuGetPackage(Packages = new[] { "LightInject" })]
    public class ContainerTests : AgentMulderTestBase<LightInjectContainerInfo>
    {
        protected override string RelativeTestDataPath => "LightInject";
    }
}
