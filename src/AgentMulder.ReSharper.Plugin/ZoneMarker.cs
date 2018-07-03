using System.Runtime.CompilerServices;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;

[assembly: InternalsVisibleTo("AgentMulder.ReSharper.Tests")]

namespace AgentMulder.ReSharper.Plugin
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone> { }
}