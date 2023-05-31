using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class CotExpr : UnaryExpr
{
    private const string OperatorFormat = "cot({0})";

    public CotExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var e = Expr.Evaluate(values);
        var res = 1 / System.Math.Tan(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var cotPow2 = this.Pow(2);
        var derivative = -(cotPow2 + 1) * Expr.Derivative(wrt);
        return derivative;
    }
}