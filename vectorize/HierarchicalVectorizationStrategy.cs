public class HierarchicalVectorizationStrategy : IVectorizationStrategy
{
    private readonly AutoTuner autoTuner;

    public HierarchicalVectorizationStrategy(AutoTuner autoTuner)
    {
        this.autoTuner = autoTuner;
    }

    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();

        if (loopInfo.NestedLoops.Any())
        {
            code.AppendLine(VectorizeNestedLoops(loopInfo, patternInfo));
        }
        else
        {
            var bestStrategy = autoTuner.TuneAndSelect(loopInfo, patternInfo);
            code.AppendLine(bestStrategy.Apply(loopInfo, patternInfo));
        }

        return code.ToString();
    }

    private string VectorizeNestedLoops(LoopInfo outerLoop, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        code.AppendLine($"for (int {outerLoop.LoopVariable} = 0; {outerLoop.LoopVariable} < {outerLoop.UpperBound}; {outerLoop.LoopVariable}++)");
        code.AppendLine("{");

        foreach (var innerLoop in outerLoop.NestedLoops)
        {
            var innerStrategy = autoTuner.TuneAndSelect(innerLoop, patternInfo);
            code.AppendLine(innerStrategy.Apply(innerLoop, patternInfo));
        }

        code.AppendLine("}");
        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        // Estimate performance based on the nested structure
        return (double)complexity / (inputSize * Math.Log(inputSize));
    }
}