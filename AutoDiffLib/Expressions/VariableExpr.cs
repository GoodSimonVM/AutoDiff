using System;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public class VariableExpr : Expr
{
    internal VariableExpr(string name)
        : base(null)
    {
        Name = name;
    }

    public string Name { get; }

    public double Value { get; set; }

    public override double Evaluate()
    {
        return Value;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        ConstantExpr c = wrt == this ? 1 : 0;
        return c;
    }

    public override string ToString()
    {
        return Name;
    }

    public override string ToString(string? format)
    {
        return Name;
    }

    public override string ToString(IFormatProvider? provider)
    {
        return Name.ToString(provider);
    }

    public override string ToString(string? format, IFormatProvider? provider)
    {
        return Name.ToString(provider);
    }
}