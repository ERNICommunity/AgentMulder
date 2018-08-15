// Patterns: 1
// Matches: TestContext.cs
// NotMatches: Baz.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestApplication.Types;

namespace TestApplication.AspNetCore
{
    public class AddDbContext
    {
        public AddDbContext()
        {
            var container = new ServiceCollection();
            container.AddDbContext<TestContext>();
        }
    }
}