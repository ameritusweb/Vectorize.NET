using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Vectorize.NET
{
    public class LoopAnalyzer : CSharpSyntaxWalker
    {
        private readonly SemanticModel semanticModel;
        public List<LoopInfo> AnalyzedLoops { get; } = new List<LoopInfo>();

        public LoopAnalyzer(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            var loopInfo = new LoopInfo
            {
                Syntax = node,
                DependencyInfo = AnalyzeLoopDependencies(node),
                LoopVariable = ExtractLoopVariable(node),
                UpperBound = ExtractUpperBound(node),
                NestedLoops = ExtractNestedLoops(node)
            };
            AnalyzedLoops.Add(loopInfo);
            base.VisitForStatement(node);
        }

        private LoopDependencyInfo AnalyzeLoopDependencies(ForStatementSyntax node)
        {
            var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(node);
            var dependencyInfo = new LoopDependencyInfo();

            foreach (var symbol in dataFlowAnalysis.WrittenInside)
            {
                if (dataFlowAnalysis.ReadInside.Contains(symbol))
                {
                    var dependency = AnalyzeDependency(node, symbol);
                    dependencyInfo.Dependencies.Add(dependency);
                }
            }

            return dependencyInfo;
        }

        private DependencyInfo AnalyzeDependency(ForStatementSyntax node, ISymbol symbol)
        {
            // Implement more sophisticated dependency analysis
            // This is a simplified version
            return new DependencyInfo
            {
                Symbol = symbol,
                Type = IsLoopCarriedDependency(node, symbol) ? DependencyType.LoopCarried : DependencyType.IntraIteration
            };
        }

        private bool IsLoopCarriedDependency(ForStatementSyntax node, ISymbol symbol)
        {
            // Simplified check for loop-carried dependency
            return node.ToString().Contains($"{symbol.Name} = {symbol.Name}");
        }

        private string ExtractLoopVariable(ForStatementSyntax node)
        {
            return node.Declaration?.Variables.FirstOrDefault()?.Identifier.Text;
        }

        private string ExtractUpperBound(ForStatementSyntax node)
        {
            if (node.Condition is BinaryExpressionSyntax condition)
            {
                return condition.Right.ToString();
            }
            return null;
        }

        private List<LoopInfo> ExtractNestedLoops(ForStatementSyntax node)
        {
            var nestedLoops = new List<LoopInfo>();
            foreach (var nestedLoop in node.Statement.DescendantNodes().OfType<ForStatementSyntax>())
            {
                nestedLoops.Add(new LoopInfo
                {
                    Syntax = nestedLoop,
                    DependencyInfo = AnalyzeLoopDependencies(nestedLoop),
                    LoopVariable = ExtractLoopVariable(nestedLoop),
                    UpperBound = ExtractUpperBound(nestedLoop)
                });
            }
            return nestedLoops;
        }
    }
}