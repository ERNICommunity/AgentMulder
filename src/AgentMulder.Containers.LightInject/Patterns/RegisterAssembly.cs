using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.ReSharper.Domain.Elements.Modules;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public sealed class RegisterAssembly : RegistrationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$container$.RegisterAssembly($assembly$)",
                new ExpressionPlaceholder("container", "global::LightInject.ServiceContainer", true),
                new ArgumentPlaceholder("assembly", 1, 1));


        public RegisterAssembly()
            : base(pattern)
        {
        }
        
        private IEnumerable<IModule> GetTargetModule(IStructuralMatchResult match)
        {
            var argument = match.GetMatchedElement("assembly") as ICSharpArgument;

            if (argument == null)
            {
                yield break;
            }

            IModule module = ModuleExtractor.GetTargetModule(argument.Value);

            if (module != null)
            {
                yield return module;
            }
            else
            {
                var value = argument.Value;

                if (value?.ConstantValue == null || !value.ConstantValue.IsString())
                {
                    yield break;
                }

                var patternToLook = Convert.ToString(value.ConstantValue.Value)
                    .Replace("*", "")
                    .Replace(".dll", "")
                    .Replace(".exe", "");

                ISolution solution = value.GetSolution();

                foreach (var project in solution.GetAllProjects().Where(project => project.Name.Contains(patternToLook)))
                {
                    yield return project;
                }
            }
        }
        

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IStructuralMatchResult match = Match(registrationRootElement);

            if (match.Matched)
            {
                IEnumerable<IModule> modules = GetTargetModule(match);

                foreach (var module in modules)
                {
                    yield return new ModuleBasedOnRegistration(registrationRootElement, module);
                }
            }
        }
    }
}
