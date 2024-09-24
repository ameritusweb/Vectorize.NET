Program.cs

Main entry point for the application


Vectorizer.cs

Main class that orchestrates the vectorization process


LoopAnalyzer.cs

Analyzes loops and their dependencies


PatternRecognizer.cs

Recognizes common loop patterns


AutoTuner.cs

Selects the best vectorization strategy based on performance estimates


VectorizationFeedback.cs

Provides feedback on the vectorization process


TensorShapeManager.cs

Manages tensor shapes throughout the vectorization process


VectorizedCodeGenerator.cs

Generates vectorized code using PradOp operations


OperationOptimizer.cs

Optimizes and reorders operations


HardwareInfo.cs

Contains information about the hardware capabilities


LoopInfo.cs

Represents information about a loop


DependencyInfo.cs

Represents dependency information for a loop


PatternInfo.cs

Represents recognized pattern information


Strategies/

IVectorizationStrategy.cs (interface)
FullVectorizationStrategy.cs
PartialVectorizationStrategy.cs
LoopUnrollingStrategy.cs
HierarchicalVectorizationStrategy.cs
PrefixSumStrategy.cs
FibonacciVectorizationStrategy.cs
MatrixMultiplicationStrategy.cs


Operations/

OperationNode.cs
PrefixSumOperation.cs
CustomOperation.cs


Exceptions/

VectorizationException.cs


Utils/

RoslynExtensions.cs (helper methods for working with Roslyn)


Models/

ComplexityLevel.cs (enum)
PatternType.cs (enum)
DependencyType.cs (enum)