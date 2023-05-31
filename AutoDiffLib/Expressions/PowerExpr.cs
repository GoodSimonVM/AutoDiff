using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class PowerExpr : BinaryExpr
{
    private const string OperatorFormat = "({0}^{1})";

    public PowerExpr(Expr x, Expr n) : base(OperatorFormat, x, n)
    {
    }

    public Expr Base => Left;
    public Expr Power => Right;

    public override double Evaluate(IDictionary<string, double> values)
    {
        var x = Base.Evaluate(values);
        var n = Power.Evaluate(values);
        var res = System.Math.Pow(x, n);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var b = Base;
        var p = Power;

        var db = b.Derivative(wrt);
        var dp = p.Derivative(wrt);

        var p1 = p - 1;

        var bp1 = b.Pow(p1);
        var l =(p * bp1) * db;

        if (b is not ConstantExpr && p is ConstantExpr)
            return l;

        var r = Math.Ln(b) * b.Pow(p) * dp;

        if (b is ConstantExpr && p is not ConstantExpr)
        {
            return r;
        }

        var derivative = l + r;
        return derivative;
    }
}