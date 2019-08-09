using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.Containers.CastleWindsor;
using AgentMulder.Containers.CastleWindsor.Providers;
using AgentMulder.ReSharper.Domain.Containers;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.Windsor
{
    [TestWithNuGetPackage(Packages = new[] { "Castle.Windsor/3.3.0", "Castle.Core/3.3.0" })]
    public class ClassesTests : AgentMulderTestBase<WindsorContainerInfo>
    {
        private const string RelativePath = @"Windsor\TestCases";

        protected override string RelativeTestDataPath => RelativePath;

        protected override IContainerInfo ContainerInfo
        {
            get
            {
                return new WindsorContainerInfo(new[]
                {
                    new ClassesRegistrationProvider(new BasedOnRegistrationProvider())
                });
            }
        }

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