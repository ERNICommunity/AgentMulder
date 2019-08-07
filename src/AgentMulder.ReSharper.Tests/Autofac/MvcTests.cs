using System.Collections;
using System.IO;
using System.Linq;
using AgentMulder.Containers.Autofac;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

namespace AgentMulder.ReSharper.Tests.Autofac
{
    [TestWithNuGetPackage(Packages = new[] { "Autofac/3.5.2", "Autofac.Mvc5/3.3.4", "Microsoft.AspNet.WebApi.Core/5.2.3" })]
    public class MvcTests : AgentMulderTestBase<AutofacContainerInfo>
    {
        private const string RelativePath = @"Autofac\Mvc";

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