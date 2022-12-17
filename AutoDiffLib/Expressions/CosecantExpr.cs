namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class CosecantExpr : UnaryExpr
{
    private const string OperatorFormat = "csc({0})";

    public CosecantExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var e = Expr.Evaluate();
        var res = 1 / System.Math.Sin(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var sin = Math.Sin(Expr);
        var dSin = sin.Derivative(wrt);
        var sinPow2 = sin.Pow(2);
        var derivative = -dSin / sinPow2;
        return derivative;
    }
}