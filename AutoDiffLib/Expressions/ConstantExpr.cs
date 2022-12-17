using System;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public class ConstantExpr : Expr
{
    private ConstantExpr(double value) :
        base(null)
    {
        Value = value;
    }

    public double Value { get; protected set; }

    public static implicit operator ConstantExpr(int value)
    {
        return new ConstantExpr(value);
    }

    public static implicit operator ConstantExpr(double value)
    {
        return new ConstantExpr(value);
    }

    public override double Evaluate()
    {
        return Value;
    }

    public override Expr Derivative(VariableExpr wrt)
    {
        return new ConstantExpr(0);
    }

    public bool IsZero()
    {
        return Value == 0d;
    }

    public bool IsOne()
    {
        return System.Math.Abs(Value - 1d) < double.Epsilon;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override string ToString(string? format)
    {
        return Value.ToString(format);
    }

    public override string ToString(IFormatProvider? provider)
    {
        return Value.ToString(provider);
    }

    public override string ToString(string? format, IFormatProvider? provider)
    {
        return Value.ToString(format, provider);
    }
}