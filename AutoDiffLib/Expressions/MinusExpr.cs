namespace GoodSimonVM.AutoDiffLib.Expressions;

internal class MinusExpr : MulExpr
{
    public MinusExpr(Expr expr) : base(-1, expr)
    {
    }
}