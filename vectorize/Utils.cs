using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Vectorize.NET.Utils
{
    public static class RoslynExtensions
    {
        public static IEnumerable<T> DescendantNodesOfType<T>(this SyntaxNode node) where T : SyntaxNode
        {
            return node.DescendantNodes().OfType<T>();
        }

        public static bool IsNumericType(this TypeInfo typeInfo)
        {
            if (typeInfo.Type == null)
                return false;

            switch (typeInfo.Type.SpecialType)
            {
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsLoopIndexVariable(this ISymbol symbol, ForStatementSyntax forStatement)
        {
            var declaration = forStatement.Declaration;
            if (declaration == null)
                return false;

            var variables = declaration.Variables;
            return variables.Any(v => v.Identifier.Text == symbol.Name);
        }

        public static ExpressionSyntax GetLoopBound(this ForStatementSyntax forStatement)
        {
            if (forStatement.Condition is BinaryExpressionSyntax condition)
            {
                return condition.Right;
            }
            return null;
        }

        public static string GetOperationFromBinaryExpression(this BinaryExpressionSyntax binaryExpression)
        {
            switch (binaryExpression.Kind())
            {
                case SyntaxKind.AddExpression:
                    return "Add";
                case SyntaxKind.SubtractExpression:
                    return "Sub";
                case SyntaxKind.MultiplyExpression:
                    return "Mul";
                case SyntaxKind.DivideExpression:
                    return "Div";
                // Add more cases as needed
                default:
                    return "Unknown";
            }
        }
    }
}