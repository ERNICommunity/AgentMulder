using System.Collections.Generic;
using System.Linq;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentMulder.Containers.LightInject
{
    public abstract class LightInjectPatternBase : RegistrationPatternBase
    {
        protected LightInjectPatternBase(IStructuralSearchPattern pattern)
        : base(pattern)
        {
        }

        protected IEnumerable<IDeclaredType> GetTypesFromLambda(ILambdaExpression lambdaExpression)
        {
            // match lambda expressions
            if (lambdaExpression != null)
            {
                IDeclaredType declaredType = null;
                if (lambdaExpression.BodyBlock != null)
                {
                    // lambda with statement body
                    var returnTypes =
                        lambdaExpression.BodyBlock.Descendants<IReturnStatement>()
                            .ToEnumerable()
                            .Select(_ => _?.Value?.GetExpressionType() as IDeclaredType)
                            .Where(_ => _ != null && _.Classify == TypeClassification.REFERENCE_TYPE);

                    foreach (var returnType in returnTypes)
                    {
                        yield return returnType;
                    }
                }
                else if (lambdaExpression.BodyExpression != null)
                {
                    // lambda with expression body
                    declaredType = lambdaExpression.BodyExpression.GetExpressionType() as IDeclaredType;
                }

                if (declaredType != null && declaredType.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    yield return declaredType;
                }
            }
        }
    }
}
