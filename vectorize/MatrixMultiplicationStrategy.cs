using System;
using System.Text;

public class MatrixMultiplicationStrategy : IVectorizationStrategy
{
    public string Apply(LoopInfo loopInfo, PatternInfo patternInfo)
    {
        var code = new StringBuilder();
        code.AppendLine("// Vectorized matrix multiplication");
        code.AppendLine("var result = pradOp.MatMul(matrixB);");
        return code.ToString();
    }

    public double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0)
    {
        double matrixDim = Math.Sqrt(inputSize);
        double operations = matrixDim * matrixDim * matrixDim;
        double simdEffect = simdWidth > 0 ? simdWidth : 1;
        double cacheEffect = cacheSize > 0 ? Math.Log(cacheSize) : 1;
        
        return operations / (simdEffect * cacheEffect);
    }
}