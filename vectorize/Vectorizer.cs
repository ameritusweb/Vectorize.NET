public class Vectorizer
{
    private readonly PatternRecognizer patternRecognizer;
    private readonly AutoTuner autoTuner;
    private readonly VectorizationFeedback feedback;
    private readonly TensorShapeManager shapeManager;

    public Vectorizer(HardwareInfo hardwareInfo)
    {
        patternRecognizer = new PatternRecognizer();
        autoTuner = new AutoTuner(hardwareInfo);
        feedback = new VectorizationFeedback();
        shapeManager = new TensorShapeManager();
    }

    public string VectorizeFunction(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        var loopAnalyzer = new LoopAnalyzer(semanticModel);
        loopAnalyzer.Visit(method);

        var code = new StringBuilder();
        code.AppendLine($"public {method.ReturnType} {method.Identifier}({string.Join(", ", method.ParameterList.Parameters)})");
        code.AppendLine("{");
        code.AppendLine("    var pradOp = new PradOp(input);");

        foreach (var loop in loopAnalyzer.AnalyzedLoops)
        {
            var gatherNdInfo = patternRecognizer.DetectGatherNdOpportunity(loop.Syntax, semanticModel);

            if (gatherNdInfo.CanApplyGatherNd)
            {
                feedback.AddMessage("Applying GatherNd optimization");
                var gatherNdStrategy = new GatherNdStrategy();
                code.AppendLine(gatherNdStrategy.Apply(loop, gatherNdInfo));
            }
            else
            {
                // Existing logic for other vectorization strategies
                var tilingInfo = patternRecognizer.DetectTilingOpportunity(loop.Syntax, semanticModel);
                if (tilingInfo.CanApplyTiling)
                {
                    feedback.AddMessage("Applying advanced tiling strategy");
                    var tilingStrategy = new TilingStrategy();
                    code.AppendLine(tilingStrategy.Apply(loop, tilingInfo));
                }
                else if (patternRecognizer.CanApplyBroadcasting(loop))
                {
                    feedback.AddMessage("Applying broadcasting optimization");
                    var broadcastingStrategy = new BroadcastingStrategy();
                    code.AppendLine(broadcastingStrategy.Apply(loop, patternInfo));
                }
                else
                {
                    // Existing logic for other vectorization strategies
                    var patternInfo = patternRecognizer.RecognizePattern(loop.Syntax, loop.DependencyInfo);
                    feedback.AddMessage($"Pattern recognized: {patternInfo.Type}");

                    shapeManager.UpdateShape(loop);

                    var bestStrategy = autoTuner.TuneAndSelect(loop, patternInfo);
                    feedback.AddMessage($"Selected strategy: {bestStrategy.GetType().Name}");

                    code.AppendLine(bestStrategy.Apply(loop, patternInfo, shapeManager));
                }
            }
        }

        code.AppendLine("    return result;");
        code.AppendLine("}");

        feedback.PrintFeedback();
        return code.ToString();
    }
}