using System;
using System.Collections.Generic;
using GoodSimonVM.AutoDiffLib.Exceptions;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public class VariableExpr : Expr
{
    internal VariableExpr(string name)
        : base(null)
    {
        Name = name;
    }

    public string Name { get; }

    public override double Evaluate(IDictionary<string, double> values)
    {
        if (values.TryGetValue(Name, out var value))
            return value;
        throw new ArgumentNotFound(Name);
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