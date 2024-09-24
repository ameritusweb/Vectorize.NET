public class ControlFlowAnalyzer
{
    public List<ConditionalInfo> Conditionals { get; } = new List<ConditionalInfo>();
    public bool HasComplexControlFlow { get; private set; }

    public ControlFlowAnalysisResult Analyze(SyntaxNode node)
    {
        Visit(node);
        return new ControlFlowAnalysisResult
        {
            Conditionals = Conditionals,
            HasComplexControlFlow = HasComplexControlFlow
        };
    }

    private void Visit(SyntaxNode node)
    {
        switch (node)
        {
            case IfStatementSyntax ifStatement:
                AnalyzeIfStatement(ifStatement);
                break;
            case SwitchStatementSyntax switchStatement:
                AnalyzeSwitchStatement(switchStatement);
                break;
            case ForStatementSyntax forStatement:
                AnalyzeForStatement(forStatement);
                break;
            case WhileStatementSyntax whileStatement:
                AnalyzeWhileStatement(whileStatement);
                break;
            case DoStatementSyntax doStatement:
                AnalyzeDoStatement(doStatement);
                break;
        }

        foreach (var child in node.ChildNodes())
        {
            Visit(child);
        }
    }

    private void AnalyzeIfStatement(IfStatementSyntax ifStatement)
    {
        if (IsVectorizableConditional(ifStatement))
        {
            Conditionals.Add(new ConditionalInfo
            {
                Condition = ifStatement.Condition.ToString(),
                ThenStatement = ifStatement.Statement.ToString(),
                ElseStatement = ifStatement.Else?.Statement.ToString()
            });
        }
        else
        {
            HasComplexControlFlow = true;
        }
    }

    private void AnalyzeSwitchStatement(SwitchStatementSyntax switchStatement)
    {
        // Switch statements are generally considered complex control flow
        HasComplexControlFlow = true;

        // However, we can check if it's a simple switch that could be vectorized
        if (IsVectorizableSwitch(switchStatement))
        {
            Conditionals.Add(new ConditionalInfo
            {
                Condition = switchStatement.Expression.ToString(),
                ThenStatement = string.Join(Environment.NewLine, switchStatement.Sections.Select(s => s.ToString())),
                ElseStatement = null
            });
        }
    }

    private void AnalyzeForStatement(ForStatementSyntax forStatement)
    {
        // For now, we'll consider nested loops as complex control flow
        if (forStatement.DescendantNodes().OfType<ForStatementSyntax>().Any())
        {
            HasComplexControlFlow = true;
        }
    }

    private void AnalyzeWhileStatement(WhileStatementSyntax whileStatement)
    {
        // While loops are generally considered complex control flow
        HasComplexControlFlow = true;
    }

    private void AnalyzeDoStatement(DoStatementSyntax doStatement)
    {
        // Do-while loops are generally considered complex control flow
        HasComplexControlFlow = true;
    }

    private bool IsVectorizableConditional(IfStatementSyntax ifStatement)
    {
        // Check if the condition is a simple comparison
        if (ifStatement.Condition is BinaryExpressionSyntax binaryExpression)
        {
            var kind = binaryExpression.Kind();
            if (kind == SyntaxKind.EqualsExpression ||
                kind == SyntaxKind.NotEqualsExpression ||
                kind == SyntaxKind.LessThanExpression ||
                kind == SyntaxKind.LessThanOrEqualExpression ||
                kind == SyntaxKind.GreaterThanExpression ||
                kind == SyntaxKind.GreaterThanOrEqualExpression)
            {
                // Check if the body contains only simple assignments
                return ContainsOnlySimpleAssignments(ifStatement.Statement) &&
                       (ifStatement.Else == null || ContainsOnlySimpleAssignments(ifStatement.Else.Statement));
            }
        }
        return false;
    }

    private bool IsVectorizableSwitch(SwitchStatementSyntax switchStatement)
    {
        // Check if all sections contain only simple assignments
        return switchStatement.Sections.All(section =>
            section.Statements.All(statement => IsSimpleAssignment(statement)));
    }

    private bool ContainsOnlySimpleAssignments(StatementSyntax statement)
    {
        if (statement is BlockSyntax block)
        {
            return block.Statements.All(s => IsSimpleAssignment(s));
        }
        return IsSimpleAssignment(statement);
    }

    private bool IsSimpleAssignment(StatementSyntax statement)
    {
        if (statement is ExpressionStatementSyntax expressionStatement)
        {
            return expressionStatement.Expression is AssignmentExpressionSyntax;
        }
        return false;
    }
}