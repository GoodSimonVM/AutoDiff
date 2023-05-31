using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class TanExpr : UnaryExpr
{
    private const string OperatorFormat = "tan({0})";

    public TanExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var e = Expr.Evaluate(values);
        var res = System.Math.Tan(e);
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var tanPow2 = this.Pow(2);
        var derivative = (tanPow2 + 1) * Expr.Derivative(wrt);
        return derivative;
    }
}