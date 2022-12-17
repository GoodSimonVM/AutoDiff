namespace GoodSimonVM.AutoDiffLib.Expressions;

public static class Math
{
    public static Expr Exp(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Exp(e.Evaluate());

        return Pow(System.Math.E, e);
    }


    public static Expr Cot(Expr e)
    {
        if (e is ConstantExpr)
            return 1 / System.Math.Tan(e.Evaluate());

        return new CotExpr(e);
    }


    public static Expr Tan(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Tan(e.Evaluate());

        return new TanExpr(e);
    }

    public static Expr Sin(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Sin(e.Evaluate());

        return new SinExpr(e);
    }

    public static Expr Cos(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Cos(e.Evaluate());

        return new CosExpr(e);
    }

    public static Expr Abs(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Abs(e.Evaluate());

        return new AbsExpr(e);
    }

    public static Expr Sgn(Expr e)
    {
        if (e is ConstantExpr)
            return System.Math.Sign(e.Evaluate());

        return new SignExpr(e);
    }

    public static Expr Ln(Expr a)
    {
        if (a is ConstantExpr)
            return System.Math.Log(a.Evaluate());
        return new LnExpr(a);
    }

    public static Expr Lg(Expr a)
    {
        if (a is ConstantExpr)
            return System.Math.Log10(a.Evaluate());
        return new LgExpr(a);
    }

    public static Expr Log(Expr a, Expr b)
    {
        return b switch
        {
            ConstantExpr _ when b.Evaluate() == 10d => Lg(a),
            ConstantExpr _ when b.Evaluate() == System.Math.E => Ln(a),
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