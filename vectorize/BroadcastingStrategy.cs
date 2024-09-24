public class BroadcastingStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        var arrayAccesses = loopInfo.Syntax.DescendantNodes()
            .OfType<ElementAccessExpressionSyntax>()
            .Select(access => access.Expression.ToString())
            .Distinct()
            .ToList();

        string largeArray = arrayAccesses.First(access => access.Contains(loopInfo.LoopVariable));
        string smallArray = arrayAccesses.First(access => !access.Contains(loopInfo.LoopVariable));

        code.AppendLine($"var broadcastedSmall = pradOp.BroadcastTo({smallArray}, {largeArray}.Shape);");
        code.AppendLine($"var result = pradOp.ElementwiseOperation({largeArray}, broadcastedSmall, (a, b) => /* operation */);");

        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        // Broadcasting typically improves performance by reducing loop iterations
        return complexity * (inputSize / simdWidth) * 0.5; // Simplified model
    }
}