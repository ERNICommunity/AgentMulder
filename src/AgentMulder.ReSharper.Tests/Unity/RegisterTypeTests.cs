using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.Containers.Unity;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.Unity
{
    [TestWithNuGetPackage(Packages = new[] { "Unity/3.5.1404", "CommonServiceLocator/1.2.0" })]
    public class RegisterTypeTests : AgentMulderTestBase<UnityContainerInfo>
    {
        private const string RelativePath = "Unity";

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