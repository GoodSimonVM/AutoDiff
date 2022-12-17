namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class AddExpr : BinaryExpr
{
    private const string OperatorFormat = "({0} + {1})";

    public AddExpr(Expr l, Expr r) : base(OperatorFormat, l, r)
    {
    }

    public override double Evaluate()
    {
        var l = Left.Evaluate();
        var r = Right.Evaluate();
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