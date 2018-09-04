using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.CSharp.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace AgentMulder.Containers.Autofac.Patterns
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    internal sealed class RegisterTypeGeneric : ComponentImplementationPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new CSharpStructuralSearchPattern("$builder$.RegisterType<$type$>()",
                new ExpressionPlaceholder("builder", "global::Autofac.ContainerBuilder", true),
                new TypePlaceholder("type"));

        public RegisterTypeGeneric()
            : base(pattern, "type")
        {
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            var registrations = new List<IComponentRegistration>();

            var parentInvocations = GetParentInvocations(registrationRootElement).ToList();
            foreach (var registration in base.GetComponentRegistrations(registrationRootElement).Cast<ComponentRegistration>())
            {
                if (!parentInvocations.Any())
                {
                    registration.Implementation = registration.ServiceType;
                    registrations.Add(registration);
                    continue;
                }

                var implementationType = registration.ServiceType;
                foreach (var parentInvocation in parentInvocations)
                {
                    var name = parentInvocation.InvocationExpressionReference.GetName();
                    if (name == "AsSelf")
                    {
                        registrations.Add(new ComponentRegistration(registrationRootElement, implementationType, implementationType));
                    }

                    if (name == "AsImplementedInterfaces")
                    {
                        var interfaces = implementationType.GetSuperTypes()
                            .Select(type => type.GetTypeElement())
                            .Where(_ => _ != null)
                            .OfType<IInterface>()
                            .Where(@interface => @interface.GetClrName().FullName != "System.IDisposable");

                        foreach (var i in interfaces)
                        {
                            registrations.Add(new ComponentRegistration(registrationRootElement, i, implementationType));
                        }
                    }

                    if (name == "As")
                    {
                        if (parentInvocation.TypeArguments.Any())
                        {
                            var serviceType = parentInvocation.TypeArguments.First().GetTypeElement();
                            registrations.Add(new ComponentRegistration(registrationRootElement, serviceType, implementationType));
                        }
                        else if (parentInvocation.ArgumentList.Arguments.Count == 1)
                        {
                            var typeOfExpression = parentInvocation.ArgumentList.Arguments[0].Expression as ITypeofExpression;
                            if (typeOfExpression != null)
                            {
                                var typeElement = (IDeclaredType)typeOfExpression.ArgumentType;

                                registrations.Add(new ComponentRegistration(registrationRootElement,
                                    typeElement.GetTypeElement(), implementationType));
                            }
                        }
                    }
                }
            }

            return registrations;
        }

        private IEnumerable<IInvocationExpression> GetParentInvocations(ITreeNode leaf)
        {
            var current = leaf;

            while (current.Parent != null)
            {
                if (current.Parent is IInvocationExpression)
                {
                    yield return (IInvocationExpression)current.Parent;
                }

                current = current.Parent;
            }
        }
    }
}