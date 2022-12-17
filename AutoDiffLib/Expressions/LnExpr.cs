namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class LnExpr : LogExpr
{
    private const string OperatorFormat = "ln({0})";

    public LnExpr(Expr x) : base(OperatorFormat, x, System.Math.E)
    {
    }

    public override double Evaluate()
    {
        var x = A.Evaluate();
        return System.Math.Log(x);
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var a = A;
        var da = a.Derivative(wrt);
        var derivative = da / a;
        return derivative;
    }
}