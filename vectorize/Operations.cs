using System;

namespace Vectorize.NET.Operations
{
    public class OperationNode
    {
        public string Operation { get; set; }
        public OperationType Type { get; set; }
        public List<string> Inputs { get; set; } = new List<string>();
        public string Output { get; set; }

        public virtual string GenerateCode()
        {
            return $"var {Output} = pradOp.{Operation}({string.Join(", ", Inputs)});";
        }
    }

    public enum OperationType
    {
        Elementwise,
        Reduction,
        Custom,
        MatrixMultiplication
    }
    
    public class CustomOperation : OperationNode
    {
        private readonly Func<List<string>, string> codeGenerator;

        public CustomOperation(string operation, List<string> inputs, Func<List<string>, string> codeGenerator)
        {
            Operation = operation;
            Type = OperationType.Custom;
            Inputs = inputs;
            Output = $"custom_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            this.codeGenerator = codeGenerator;
        }

        public override string GenerateCode()
        {
            return codeGenerator(Inputs);
        }
    }

    public class PrefixSumOperation : OperationNode
    {
        public PrefixSumOperation(string input)
        {
            Operation = "PrefixSum";
            Type = OperationType.Reduction;
            Inputs.Add(input);
            Output = $"prefixSum_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        public override string GenerateCode()
        {
            return $"var {Output} = pradOp.PrefixSum({Inputs[0]});";
        }
    }
}