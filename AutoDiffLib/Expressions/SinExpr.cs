namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class SinExpr : UnaryExpr
{
    private const string OperatorFormat = "sin({0})";

    public SinExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var e = Expr.Evaluate();
        var res = System.Math.Sin(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var derivative = Math.Cos(Expr) * Expr.Derivative(wrt);
        return derivative;
    }
}