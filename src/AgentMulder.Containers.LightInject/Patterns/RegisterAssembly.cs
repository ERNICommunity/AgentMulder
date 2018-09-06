using System;
using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Elements.Modules;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject.Patterns
{
   public abstract class RegisterAssembly: RegistrationPatternBase
    {
        protected RegisterAssembly(IStructuralSearchPattern pattern)
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

                var patternsToLook = Convert.ToString(value.ConstantValue.Value)
                    .Replace(".dll", "")
                    .Replace(".exe", "")
                    .Trim('*')
                    .Split('*');

                ISolution solution = value.GetSolution();

                var projects = solution.GetAllProjects()
                    .Where(project => patternsToLook.All(p => project.Name.Contains(p)));

                foreach (var project in projects)
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
