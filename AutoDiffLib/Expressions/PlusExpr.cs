namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class PlusExpr : UnaryExpr
{
    private const string OperatorFormat = "{0}";

    public PlusExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var res = Expr.Evaluate();
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var derivative = Expr.Derivative(wrt);
        return derivative;
    }
}