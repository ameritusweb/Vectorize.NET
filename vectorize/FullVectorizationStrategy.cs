using System;
using System.Text;

public class FullVectorizationStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        code.AppendLine($"// Full vectorization of loop: {loopInfo.LoopVariable}");
        code.AppendLine($"var result = pradOp");
        
        // Assuming the loop body involves element-wise operations
        foreach (var operation in loopInfo.Operations)
        {
            switch (operation.Type)
            {
                case OperationType.Add:
                    code.AppendLine($"    .Add({operation.Operand})");
                    break;
                case OperationType.Mul:
                    code.AppendLine($"    .Mul({operation.Operand})");
                    break;
                case OperationType.Sin:
                    code.AppendLine($"    .Then(PradOp.SinOp)");
                    break;
                case OperationType.Cos:
                    code.AppendLine($"    .Then(PradOp.CosOp)");
                    break;
                // Add more cases for other PradOp operations
            }
        }
        
        code.AppendLine($"    .Result;");
        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        return (double)complexity / (inputSize * simdWidth);
    }
}