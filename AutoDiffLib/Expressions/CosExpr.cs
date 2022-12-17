namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class CosExpr : UnaryExpr
{
    private const string OperatorFormat = "cos({0})";

    public CosExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var e = Expr.Evaluate();
        var res = System.Math.Cos(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var derivative = -(Math.Sin(Expr) * Expr.Derivative(wrt));
        return derivative;
    }
}