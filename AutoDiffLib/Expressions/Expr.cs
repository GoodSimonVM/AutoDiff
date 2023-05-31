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
        GetVariables(set);
        return set.ToList().AsReadOnly();
    }

    private void GetVariables(HashSet<VariableExpr> set)
    {
        if (this is VariableExpr variableExpr)
            set.Add(variableExpr);
        else if (SubExpressions is not null)
            foreach (var subExpression in SubExpressions)
                subExpression.GetVariables(set);
    }

    public abstract double Evaluate(IDictionary<string, double> values);
    public abstract Expr Derivative(VariableExpr wrt);

    /*public Expr ReplaceVariable(Dictionary<VariableExpr, Expr> values)
    {
        if (this is VariableExpr variableExpr && values.TryGetValue(variableExpr, out var newExpr))
            return newExpr;
        if (SubExpressions == null) return this;

        for (var i = 0; i < SubExpressions.Count; i++)
        {
            var expr = SubExpressions[i];
            newExpr = expr.ReplaceVariable(values);
            if (newExpr != expr)
            {
            }
        }
    }*/

    private static ConstantExpr? SimplifyIfConst(Expr l, Expr r)
    {
        if (l is not ConstantExpr cl || r is not ConstantExpr cr) return null;
        var res = cl.Value + cr.Value;
        return res;
    }

    public static Expr operator +(Expr l, Expr r)
    {
        var expr = SimplifyIfConst(l, r);
        if (expr != null) return expr;
        if (l is ConstantExpr cl && cl.IsZero()) return r;
        if (r is ConstantExpr cr && cr.IsZero()) return l;
        return new AddExpr(l, r);
    }

    public static Expr operator -(Expr l, Expr r)
    {
        var expr = SimplifyIfConst(l, -r);
        if (expr != null) return expr;

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
        if (e is ConstantExpr ce)
            return (-ce.Value);
        return new MinusExpr(e);
    }

    public static Expr operator *(Expr l, Expr r)
    {
        var cl = l as ConstantExpr;
        var cr = r as ConstantExpr;

        if (l == r)
            return l.Pow(2);

        if (cl != null && cr != null)
            return cl.Value * cr.Value;

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
                    var c = ce.Value * bcl.Value;
                    return c * be.Right;
                }
                case MulExpr when be.Right is ConstantExpr bcr:
                {
                    var c = ce.Value * bcr.Value;
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
            var res = cDi.Value / cDr.Value;
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