using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public abstract class Expr
{
    protected Expr(IList<Expr>? subExpressions)
    {
        SubExpressions = subExpressions == null ? null : new ReadOnlyCollection<Expr>(subExpressions);
    }

    protected ReadOnlyCollection<Expr>? SubExpressions { get; }

    public static implicit operator Expr(int value)
    {
        ConstantExpr c = value;
        return c;
    }

    public static implicit operator Expr(double value)
    {
        ConstantExpr c = value;
        return c;
    }

    public static implicit operator Expr(float value)
    {
        ConstantExpr c = value;
        return c;
    }

    public ReadOnlyCollectionOfVariables GetVariables()
    {
        var set = new HashSet<VariableExpr>();
        GetVariables(ref set);
        return set.ToList().AsReadOnly();
    }

    private void GetVariables(ref HashSet<VariableExpr> set)
    {
        if (SubExpressions == null) return;
        foreach (var subExpression in SubExpressions)
            if (subExpression is VariableExpr indep)
                set.Add(indep);
            else
                subExpression.GetVariables(ref set);
    }

    public abstract double Evaluate();
    public abstract Expr Derivative(VariableExpr wrt);

    public static Expr operator +(Expr l, Expr r)
    {
        if (l is ConstantExpr && r is ConstantExpr)
        {
            var res = l.Evaluate() + r.Evaluate();
            return res;
        }

        if (l is ConstantExpr cl && cl.IsZero()) return r;

        if (r is ConstantExpr cr && cr.IsZero()) return l;

        return new AddExpr(l, r);
    }

    public static Expr operator -(Expr l, Expr r)
    {
        if (l is ConstantExpr && r is ConstantExpr)
        {
            var res = l.Evaluate() - r.Evaluate();
            return res;
        }

        if (l is ConstantExpr cl && cl.IsZero()) return -r;

        if (r is ConstantExpr cr && cr.IsZero()) return l;

        return new SubExpr(l, r);
    }

    public static Expr operator +(Expr e)
    {
        return new PlusExpr(e);
    }

    public static Expr operator -(Expr e)
    {
        return new MinusExpr(e);
    }

    public static Expr operator *(Expr l, Expr r)
    {
        var cl = l as ConstantExpr;
        var cr = r as ConstantExpr;

        if (l == r)
            return l.Pow(2);

        if (cl != null && cr != null)
            return l.Evaluate() * r.Evaluate();

        if ((cl != null && cl.IsZero()) ||
            (cr != null && cr.IsZero()))
            return 0;

        if (cl != null && cl.IsOne())
            return r;

        if (cr != null && cr.IsOne())
            return l;

        ConstantExpr? ce = null;
        BinaryExpr? be = null;
        if (cl != null)
        {
            ce = cl;
            be = r as BinaryExpr;
        }
        else if (cr != null)
        {
            ce = cr;
            be = l as BinaryExpr;
        }

        if (ce != null && be != null)
            switch (be)
            {
                case MulExpr or DivExpr when be.Left is ConstantExpr bcl:
                {
                    var c = ce.Evaluate() * bcl.Evaluate();
                    return c * be.Right;
                }
                case MulExpr when be.Right is ConstantExpr bcr:
                {
                    var c = ce.Evaluate() * bcr.Evaluate();
                    return c * be.Left;
                }
            }

        return new MulExpr(l, r);
    }

    public static Expr operator /(Expr divisible, Expr divisor)
    {
        var cDi = divisible as ConstantExpr;

        if (cDi != null && divisor is ConstantExpr cDr)
        {
            var res = cDi.Evaluate() / cDr.Evaluate();
            return res;
        }

        if (cDi != null && cDi.IsZero()) return 0;


        return new DivExpr(divisible, divisor);
    }

    public static Expr operator ^(Expr a, Expr b)
    {
        return a.Pow(b);
    }

    public abstract string ToString(string? format);
    public abstract string ToString(IFormatProvider? provider);
    public abstract string ToString(string? format, IFormatProvider? provider);
}