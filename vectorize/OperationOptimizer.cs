using System.Collections.Generic;
using System.Linq;

namespace Vectorize.NET
{
    public class OperationOptimizer
    {
        public List<OperationNode> Optimize(List<OperationNode> operations)
        {
            var optimizedOps = new List<OperationNode>(operations);

            CombineElementwiseOperations(optimizedOps);
            ReorderOperations(optimizedOps);
            FuseOperations(optimizedOps);

            return optimizedOps;
        }

        private void CombineElementwiseOperations(List<OperationNode> operations)
        {
            for (int i = 0; i < operations.Count - 1; i++)
            {
                if (CanCombineOperations(operations[i], operations[i + 1]))
                {
                    operations[i] = CombineOperations(operations[i], operations[i + 1]);
                    operations.RemoveAt(i + 1);
                    i--; // Recheck the current index
                }
            }
        }

        private bool CanCombineOperations(OperationNode op1, OperationNode op2)
        {
            return op1.Type == OperationType.Elementwise &&
                   op2.Type == OperationType.Elementwise &&
                   op1.Output == op2.Inputs[0] &&
                   !op2.Inputs.Skip(1).Any(input => input == op1.Output);
        }

        private OperationNode CombineOperations(OperationNode op1, OperationNode op2)
        {
            return new OperationNode
            {
                Operation = $"Combined{op1.Operation}{op2.Operation}",
                Type = OperationType.Elementwise,
                Inputs = op1.Inputs.Concat(op2.Inputs.Skip(1)).ToList(),
                Output = op2.Output
            };
        }

        private void ReorderOperations(List<OperationNode> operations)
        {
            var dependencyGraph = BuildDependencyGraph(operations);
            operations.Clear();
            operations.AddRange(TopologicalSort(dependencyGraph));
        }

        private Dictionary<OperationNode, HashSet<OperationNode>> BuildDependencyGraph(List<OperationNode> operations)
        {
            var graph = new Dictionary<OperationNode, HashSet<OperationNode>>();
            foreach (var op in operations)
            {
                graph[op] = new HashSet<OperationNode>();
                foreach (var input in op.Inputs)
                {
                    var dependentOp = operations.FirstOrDefault(o => o.Output == input);
                    if (dependentOp != null)
                    {
                        graph[op].Add(dependentOp);
                    }
                }
            }
            return graph;
        }

        private List<OperationNode> TopologicalSort(Dictionary<OperationNode, HashSet<OperationNode>> graph)
        {
            var result = new List<OperationNode>();
            var visited = new HashSet<OperationNode>();

            foreach (var node in graph.Keys)
            {
                if (!visited.Contains(node))
                {
                    TopologicalSortUtil(node, visited, result, graph);
                }
            }

            result.Reverse();
            return result;
        }

        private void TopologicalSortUtil(OperationNode node, HashSet<OperationNode> visited, List<OperationNode> result, Dictionary<OperationNode, HashSet<OperationNode>> graph)
        {
            visited.Add(node);

            foreach (var dependent in graph[node])
            {
                if (!visited.Contains(dependent))
                {
                    TopologicalSortUtil(dependent, visited, result, graph);
                }
            }

            result.Add(node);
        }

        private void FuseOperations(List<OperationNode> operations)
        {
            for (int i = 0; i < operations.Count - 1; i++)
            {
                if (CanFuseOperations(operations[i], operations[i + 1]))
                {
                    operations[i] = FuseOperations(operations[i], operations[i + 1]);
                    operations.RemoveAt(i + 1);
                    i--; // Recheck the current index
                }
            }
        }

        private bool CanFuseOperations(OperationNode op1, OperationNode op2)
        {
            return (op1.Operation == "MatMul" && op2.Operation == "MatMul" && op1.Output == op2.Inputs[0]);
        }

        private OperationNode FuseOperations(OperationNode op1, OperationNode op2)
        {
            return new OperationNode
            {
                Operation = "FusedMatMul",
                Type = OperationType.Custom,
                Inputs = new List<string> { op1.Inputs[0], op1.Inputs[1], op2.Inputs[1] },
                Output = op2.Output
            };
        }
    }
}