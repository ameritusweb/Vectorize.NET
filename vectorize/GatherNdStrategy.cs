public class GatherNdStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, GatherNdInfo gatherNdInfo)
    {
        var code = new StringBuilder();

        // Generate index tensor
        code.AppendLine("var indexTensor = pradOp.Stack(new[]");
        code.AppendLine("{");
        foreach (var dimension in gatherNdInfo.IndexingInfo.Dimensions)
        {
            code.AppendLine($"    {GenerateIndexExpression(dimension, loopInfo)},");
        }
        code.AppendLine("}, axis: -1);");

        // Apply GatherNd
        code.AppendLine($"var result = pradOp.GatherNd({gatherNdInfo.TargetArray}, indexTensor);");

        return code.ToString();
    }

    private string GenerateIndexExpression(IndexDimensionInfo dimension, LoopInfo loopInfo)
    {
        switch (dimension.Type)
        {
            case IndexExpressionType.LoopVariable:
                return $"pradOp.Range(0, {loopInfo.UpperBound})";

            case IndexExpressionType.Variable:
            case IndexExpressionType.Constant:
                return $"pradOp.Full({loopInfo.UpperBound}, {dimension.Expression})";

            case IndexExpressionType.BinaryExpression:
            case IndexExpressionType.FunctionCall:
            case IndexExpressionType.Conditional:
            case IndexExpressionType.Complex:
                return $"pradOp.Range(0, {loopInfo.UpperBound}).Then(i => {dimension.Expression})";

            default:
                throw new NotSupportedException($"Unsupported index expression type: {dimension.Type}");
        }
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        // Complex indexing with GatherNd can be very efficient
        return complexity * (inputSize / simdWidth) * 0.4; // Adjusted model for complex scenarios
    }
}