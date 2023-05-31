using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class LgExpr : LogExpr
{
    private const string OperatorFormat = "lg({0})";
    private static readonly double Ln10 = System.Math.Log(10);

    public LgExpr(Expr x) : base(OperatorFormat, x, 10)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var x = X.Evaluate(values);
        return System.Math.Log(x);
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var a = X;
        var da = a.Derivative(wrt);
        var derivative = da / (Ln10 * a);
        return derivative;
    }
}