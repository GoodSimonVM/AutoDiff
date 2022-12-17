namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class SecantExpr : UnaryExpr
{
    private const string OperatorFormat = "sec({0})";

    public SecantExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var e = Expr.Evaluate();
        var res = 1 / System.Math.Cos(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var cos = Math.Cos(Expr);
        var dSin = cos.Derivative(wrt);
        var cosPow2 = cos.Pow(2);
        var derivative = -dSin / cosPow2;
        return derivative;
    }
}