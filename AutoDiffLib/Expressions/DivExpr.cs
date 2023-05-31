using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class DivExpr : BinaryExpr
{
    private const string OperatorFormat = "({0} / {1})";

    public DivExpr(Expr divisible, Expr divisor) : base(OperatorFormat, divisible, divisor)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var l = Left.Evaluate(values);
        var r = Right.Evaluate(values);
        var res = l / r;
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var l = Left;
        var r = Right;
        var dl = Left.Derivative(wrt);
        var dr = Right.Derivative(wrt);

        var dlr = dl * r;
        var ldr = l * dr;
        var divisible = dlr - ldr;
        var divisor = r * r;
        var derivative = divisible / divisor;

        return derivative;
    }
}