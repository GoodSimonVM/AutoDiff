namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class LogExpr : BinaryExpr
{
    private const string OperatorFormat = "log{1}({0})";

    protected LogExpr(string operatorFormat, Expr x, Expr @base) : base(operatorFormat, x, @base)
    {
    }

    public LogExpr(Expr x, Expr n) : base(OperatorFormat, x, n)
    {
    }

    public Expr X => Left;
    public Expr Base => Right;

    public override double Evaluate()
    {
        var a = X.Evaluate();
        var b = Base.Evaluate();
        var res = System.Math.Log(a, b);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var a = X;
        var b = Base;
        var lna = Math.Ln(a);
        var lnb = Math.Ln(b);
        var da = a.Derivative(wrt);
        var db = b.Derivative(wrt);

        var lnbPow2 = lnb.Pow(2);

        var derivative = lna * db / (b * lnbPow2) + da / (a * lnb);
        return derivative;
    }
}