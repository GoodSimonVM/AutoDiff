using System.Collections.Generic;

namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class AddExpr : BinaryExpr
{
    private const string OperatorFormat = "({0} + {1})";

    public AddExpr(Expr l, Expr r) : base(OperatorFormat, l, r)
    {
    }

    public override double Evaluate(IDictionary<string, double> values)
    {
        var l = Left.Evaluate(values);
        var r = Right.Evaluate(values);
        var res = l + r;
        return res;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        var l = Left.Derivative(wrt);
        var r = Right.Derivative(wrt);
        var derivative = l + r;
        return derivative;
    }
}