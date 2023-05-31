using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class SignExpr : UnaryExpr
{
    private const string OperatorFormat = "sgn({0})";

    public SignExpr(Expr expr) : base(OperatorFormat, expr)
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
        var signExpr = Expr / Math.Abs(Expr);
        var derivative = signExpr.Derivative(wrt);
        return derivative;
    }
}