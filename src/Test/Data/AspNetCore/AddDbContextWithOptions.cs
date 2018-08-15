// Patterns: 1
// Matches: TestContext.cs
// NotMatches: Baz.cs

using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddDbContextWithOptions
    {
        public AddDbContextWithOptions()
        {
            var container = new ServiceCollection();
            container.AddDbContext<TestContext>(o => o.EnableSensitiveDataLogging());
        }
    }
}