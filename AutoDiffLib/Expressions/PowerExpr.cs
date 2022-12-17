namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class PowerExpr : BinaryExpr
{
    private const string OperatorFormat = "({0}^{1})";

    public PowerExpr(Expr x, Expr n) : base(OperatorFormat, x, n)
    {
    }

    private Expr Base => Left;
    private Expr Power => Right;

    public override double Evaluate()
    {
        var x = Base.Evaluate();
        var n = Power.Evaluate();
        var res = System.Math.Pow(x, n);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var b = Base;
        var p = Power;

        var db = b.Derivative(wrt);
        var dp = p.Derivative(wrt);

        var l = db * (p * b.Pow(p - 1));

        if (b is not ConstantExpr && p is ConstantExpr)
            return l;

        var r = dp * b.Pow(p) * Math.Ln(b);

        if (b is ConstantExpr && p is not ConstantExpr)
            return r;

        var derivative = l + r;
        return derivative;
    }
}