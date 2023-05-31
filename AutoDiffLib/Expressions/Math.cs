namespace GoodSimonVM.AutoDiffLib.Expressions;

public static class Math
{
    public static Expr Exp(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Exp(ce.Value);

        return Pow(System.Math.E, e);
    }


    public static Expr Cot(Expr e)
    {
        if (e is ConstantExpr ce)
            return 1 / System.Math.Tan(ce.Value);

        return new CotExpr(e);
    }


    public static Expr Tan(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Tan(ce.Value);

        return new TanExpr(e);
    }

    public static Expr Sin(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Sin(ce.Value);

        return new SinExpr(e);
    }

    public static Expr Cos(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Cos(ce.Value);

        return new CosExpr(e);
    }

    public static Expr Abs(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Abs(ce.Value);

        return new AbsExpr(e);
    }

    public static Expr Sgn(Expr e)
    {
        if (e is ConstantExpr ce)
            return System.Math.Sign(ce.Value);

        return new SignExpr(e);
    }

    public static Expr Ln(Expr a)
    {
        if (a is ConstantExpr ca)
            return System.Math.Log(ca.Value);
        return new LnExpr(a);
    }

    public static Expr Lg(Expr a)
    {
        if (a is ConstantExpr ca)
            return System.Math.Log10(ca.Value);
        return new LgExpr(a);
    }

    public static Expr Log(Expr a, Expr b)
    {
        return b switch
        {
            ConstantExpr cb when System.Math.Abs(cb.Value - 10d) < double.Epsilon => Lg(a),
            ConstantExpr cb when System.Math.Abs(cb.Value - System.Math.E) < double.Epsilon => Ln(a),
            _ => new LogExpr(a, b)
        };
    }

    public static Expr Pow(this Expr b, Expr p)
    {
        if (p is ConstantExpr cb && cb.IsOne())
            return b;

        return new PowerExpr(b, p);
    }

    public static Expr Pow2(this Expr b)
    {
        return Pow(b, 2);
    }
}