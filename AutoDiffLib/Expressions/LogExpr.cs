namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class LogExpr : BinaryExpr
{
    private const string OperatorFormat = "log{1}({0})";

    protected LogExpr(string operatorFormat, Expr x, Expr n) : base(operatorFormat, x, n)
    {
    }

    public LogExpr(Expr x, Expr n) : base(OperatorFormat, x, n)
    {
    }

    protected Expr A => Left;
    protected Expr B => Right;

    public override double Evaluate()
    {
        var a = A.Evaluate();
        var b = B.Evaluate();
        var res = System.Math.Log(a, b);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var a = A;
        var b = B;
        var lna = Math.Ln(a);
        var lnb = Math.Ln(b);
        var da = a.Derivative(wrt);
        var db = b.Derivative(wrt);

        var lnbPow2 = lnb.Pow(2);

        var derivative = lna * db / (b * lnbPow2) + da / (a * lnb);
        return derivative;
    }
}