using System;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public abstract class BinaryExpr : Expr
{
    private readonly string _operatorFormat;

    protected BinaryExpr(string operatorFormat, Expr l, Expr r)
        : base(new[] {l, r})
    {
        _operatorFormat = operatorFormat;
    }

    public Expr Left => SubExpressions![0];
    public Expr Right => SubExpressions![1];

    public override string ToString()
    {
        return ToString(null, null);
    }

    public override string ToString(string? format)
    {
        return ToString(format, null);
    }

    public override string ToString(IFormatProvider? provider)
    {
        return ToString(null, provider);
    }

    public override string ToString(string? format, IFormatProvider? provider)
    {
        var left = Left.ToString(format, provider);
        var right = Right.ToString(format, provider);
        var s = string.Format(_operatorFormat, left, right);
        return s;
    }
}