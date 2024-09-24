using System;
using System.Text;

public class FibonacciVectorizationStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        code.AppendLine("// Vectorized Fibonacci sequence generation");
        code.AppendLine("var n = result.Cols;");
        code.AppendLine("pradOp.Fill(0, result);");
        code.AppendLine("if (n > 0) pradOp.Set(0, 0, result);");
        code.AppendLine("if (n > 1) pradOp.Set(1, 1, result);");
        code.AppendLine("for (int i = 2; i < n; i += 2)");
        code.AppendLine("{");
        code.AppendLine("    var prev2 = pradOp.Indexer($\":{i}\");");
        code.AppendLine("    var prev1 = pradOp.Indexer($\"1:{i+1}\");");
        code.AppendLine("    var sum = prev2.Add(prev1);");
        code.AppendLine("    pradOp.Set(sum, $\"{i}:{i+2}\", result);");
        code.AppendLine("}");
        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        return (double)inputSize / (2 * simdWidth);
    }
}