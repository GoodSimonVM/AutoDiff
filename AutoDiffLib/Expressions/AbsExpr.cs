namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class AbsExpr : UnaryExpr
{
    private const string OperatorFormat = "|{0}|";

    public AbsExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate()
    {
        var x = Expr.Evaluate();
        var abs = System.Math.Abs(x);
        return abs;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var dExpr = Expr.Derivative(wrt);
        var derivative = Math.Sgn(Expr) * dExpr;
        return derivative;
    }
}