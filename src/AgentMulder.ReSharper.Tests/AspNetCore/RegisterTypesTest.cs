using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.Containers.AspNetCore;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.AspNetCore
{
    [TestWithNuGetPackage(Packages = new[] { "Microsoft.Extensions.DependencyInjection/1.1.0" })]
    public class RegisterTypeTests : AgentMulderTestBase<AspNetCoreContainerInfo>
    {
        private const string RelativePath = "AspNetCore";

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
