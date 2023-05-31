using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class AbsExpr : UnaryExpr
{
    private const string OperatorFormat = "|{0}|";

    public AbsExpr(Expr expr) : base(OperatorFormat, expr)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var x = Expr.Evaluate(values);
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