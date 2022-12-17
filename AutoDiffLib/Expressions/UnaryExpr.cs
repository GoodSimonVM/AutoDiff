using System;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public abstract class UnaryExpr : Expr
{
    private readonly string _operatorFormat;

    protected UnaryExpr(string operatorFormat, Expr expr)
        : base(new[] {expr})
    {
        _operatorFormat = operatorFormat;
        Expr = expr;
    }

    protected Expr Expr { get; }

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
        var expr = Expr.ToString(format, provider);
        return string.Format(_operatorFormat, expr);
    }
}