using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.Containers.LightInject;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.LightInject
{
    [TestWithNuGetPackage(Packages = new[] { "LightInject" })]
    public class ContainerTests : AgentMulderTestBase<LightInjectContainerInfo>
    {
        private const string RelativePath = "LightInject";

        protected override string RelativeTestDataPath => RelativePath;

        [Test]
        [TestCaseSource(nameof(TestCases), methodParams: new object[] { RelativePath })]
        public void Test(string fileName)
        {
            TestCore(fileName);
        }

        private static IEnumerable TestCases(string relativePath)
        {
            TestUtil.SetHomeDir(typeof(AgentMulderTestBase<>).Assembly);
            var d = new DirectoryInfo(Path.Combine(
                TestUtil.GetTestDataPathBase(typeof(AgentMulderTestBase<>).Assembly).FullPath, relativePath));

            var testCaseData = d.EnumerateFiles("*.cs")
                .Select(info => new TestCaseData(info.Name))
                .ToList();

            return testCaseData;
        }
    }
}
