using System;
using System.Text;

public class PrefixSumStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        code.AppendLine($"// Prefix sum vectorization");
        code.AppendLine($"var result = pradOp.CumSum();");
        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        return Math.Log(inputSize) / simdWidth;
    }
}