using System;
using System.Linq;
using System.Text;

namespace Vectorize.NET
{
    public class VectorizedCodeGenerator
    {
        private readonly TensorShapeManager shapeManager;
        private readonly OperationOptimizer optimizer;

        public VectorizedCodeGenerator(TensorShapeManager shapeManager, OperationOptimizer optimizer)
        {
            this.shapeManager = shapeManager;
            this.optimizer = optimizer;
        }

        public string GenerateVectorizedCode(LoopInfo loopInfo, PatternInfo patternInfo, IVectorizationStrategy strategy)
        {
            var operations = strategy.GenerateOperations(loopInfo, patternInfo);
            var optimizedOperations = optimizer.Optimize(operations);

            var code = new StringBuilder();
            foreach (var op in optimizedOperations)
            {
                code.AppendLine(GenerateOperation(op));
            }

            return code.ToString();
        }

        private string GenerateOperation(OperationNode op)
        {
            switch (op.Operation)
            {
                case "Add":
                case "Sub":
                case "Mul":
                case "Div":
                    return GenerateBinaryOperation(op);
                case "Sin":
                case "Cos":
                case "Exp":
                    return GenerateUnaryOperation(op);
                case "MatMul":
                    return GenerateMatrixMultiplication(op);
                case "Indexer":
                    return GenerateIndexer(op);
                default:
                    throw new NotSupportedException($"Operation {op.Operation} is not supported.");
            }
        }

        private string GenerateBinaryOperation(OperationNode op)
        {
            var shape = shapeManager.GetShape(op.Output);
            return $"var {op.Output} = pradOp.{op.Operation}({string.Join(", ", op.Inputs)}).Reshape({string.Join(", ", shape.Dimensions)});";
        }

        private string GenerateUnaryOperation(OperationNode op)
        {
            var shape = shapeManager.GetShape(op.Output);
            return $"var {op.Output} = pradOp.{op.Operation}({op.Inputs[0]}).Reshape({string.Join(", ", shape.Dimensions)});";
        }

        private string GenerateMatrixMultiplication(OperationNode op)
        {
            return $"var {op.Output} = pradOp.MatMul({op.Inputs[0]}, {op.Inputs[1]});";
        }

        private string GenerateIndexer(OperationNode op)
        {
            return $"var {op.Output} = pradOp.Indexer({op.Inputs[0]}, {string.Join(", ", op.Inputs.Skip(1))});";
        }
    }
}