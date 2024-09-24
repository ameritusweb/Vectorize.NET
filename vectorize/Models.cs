public class HardwareInfo
{
    public int SimdWidth { get; set; }
    public int CacheSize { get; set; }
    // Add more hardware-specific information
}

public class LoopInfo
{
    public ForStatementSyntax Syntax { get; set; }
    public LoopDependencyInfo DependencyInfo { get; set; }
    public string LoopVariable { get; set; }
    public string UpperBound { get; set; }
    public List<LoopInfo> NestedLoops { get; set; } = new List<LoopInfo>();
    public int InputSize { get; set; }
    public bool IsParallelizable => DependencyInfo.Dependencies.TrueForAll(d => d.Type != DependencyType.LoopCarried);
}

public class LoopDependencyInfo
{
    public List<DependencyInfo> Dependencies { get; } = new List<DependencyInfo>();
}

public class DependencyInfo
{
    public ISymbol Symbol { get; set; }
    public DependencyType Type { get; set; }
}

public enum DependencyType
{
    Independent,
    LoopCarried,
    IntraIteration,
    Reduction
}

public class PatternInfo
{
    public PatternType Type { get; }
    public PatternDetails Details { get; }

    public PatternInfo(PatternType type, PatternDetails details)
    {
        Type = type;
        Details = details;
    }
}

public abstract class PatternDetails { }

public class MatrixMultiplicationDetails : PatternDetails
{
    public string OuterLoopVariable { get; set; }
    public string InnerLoopVariable { get; set; }
    // Add more relevant details
}

public interface IVectorizationStrategy
{
    double EstimatePerformance(int complexity, int inputSize, int simdWidth, int cacheSize = 0);
    string Apply(LoopInfo loopInfo, PatternInfo patternInfo);
}

public class GatherNdInfo
{
    public bool CanApplyGatherNd { get; set; }
    public string TargetArray { get; set; }
    public IndexingInfo IndexingInfo { get; set; }
}

public class IndexingInfo
{
    public bool IsComplex { get; set; }
    public List<IndexDimensionInfo> Dimensions { get; set; } = new List<IndexDimensionInfo>();
}

public class IndexDimensionInfo
{
    public IndexExpressionType Type { get; set; }
    public string Expression { get; set; }
    public List<IndexDimensionInfo> Subexpressions { get; set; }
}

public class TilingInfo
{
    public bool CanApplyTiling { get; set; }
    public List<AccessPattern> TilingPatterns { get; set; } = new List<AccessPattern>();
}

public class AccessPattern
{
    public string ArrayName { get; set; }
    public List<DimensionAccess> Dimensions { get; set; }
}

public class DimensionAccess
{
    public AccessType Type { get; set; }
    public object ConstantValue { get; set; }
    public string Expression { get; set; }
    public LoopStructure LoopInfo { get; set; }
}

public class LoopStructure
{
    public string Variable { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public string Step { get; set; }
}

public class MatrixMultiplicationDetails : PatternDetails
{
    public string OuterLoopVariable { get; set; }
    public string InnerLoopVariable { get; set; }
}

public class RunningSumDetails : PatternDetails
{
    public string AccumulatorVariable { get; set; }
}

public enum PatternType
{
    Unknown,
    RunningSum,
    MatrixMultiplication,
    FibonacciSequence,
    // Add more patterns as needed
}

public class ControlFlowAnalysisResult
{
    public List<ConditionalInfo> Conditionals { get; set; }
    public bool HasComplexControlFlow { get; set; }
}

public class ConditionalInfo
{
    public string Condition { get; set; }
    public string ThenStatement { get; set; }
    public string ElseStatement { get; set; }
}