using AgentMulder.Containers.AspNetCore;

namespace AgentMulder.ReSharper.Tests.AspNetCore
{
    [TestWithNuGetPackage(Packages = new[] { "Microsoft.Extensions.DependencyInjection/1.1.0" })]
    public class RegisterTypeTests : AgentMulderTestBase<AspNetCoreContainerInfo>
    {
        protected override string RelativeTestDataPath => "AspNetCore";
    }
}
