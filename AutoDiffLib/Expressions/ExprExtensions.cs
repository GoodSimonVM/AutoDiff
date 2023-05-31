using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodSimonVM.AutoDiffLib.Expressions;

public static class ExprExtensions
{
    /// <summary>
    /// Take expression derivatives by all used variables
    /// </summary>
    /// <summary xml:lang="ru">
    /// Берёт производные по всем переменным  
    /// </summary>
    /// <returns></returns>
    public static ReadOnlyCollectionOfExpressions Derivatives(this Expr expr)
    {
        var vars = expr.GetVariables();
        var derivatives = new List<Expr>(vars.Count);
        foreach (var var in vars)
        {
            var derivative = expr.Derivative(var);
            derivatives.Add(derivative);
        }

        return derivatives.AsReadOnly();
    }

    public static ReadOnlyCollectionOfExpressions Grad(this Expr expr)
    {
        var vars = expr.GetVariables();
        return expr.Grad(vars);
    }

    public static ReadOnlyCollectionOfExpressions Grad(this Expr expr, IEnumerable<VariableExpr> wrts)
    {
        var derivatives = new List<Expr>();
        foreach (var var in wrts)
        {
            var derivative = expr.Derivative(var);
            derivatives.Add(derivative);
        }

        return derivatives.AsReadOnly();
    }

    public static ReadOnlyCollectionOfReadOnlyCollectionOfExpressions Hess(this Expr expr)
    {
        var vars = expr.GetVariables();
        return expr.Hess(vars);
    }

    public static ReadOnlyCollectionOfReadOnlyCollectionOfExpressions Hess(
        this Expr expr,
        IEnumerable<VariableExpr> wrts)
    {
        var hessian = new List<ReadOnlyCollection<Expr>>();
        foreach (var var1 in wrts)
        {
            var d1 = expr.Derivative(var1);
            var d2s = new List<Expr>();
            foreach (var var2 in wrts)
            {
                var d2 = d1.Derivative(var2);
                d2s.Add(d2);
            }

            hessian.Add(d2s.AsReadOnly());
        }

        return hessian.AsReadOnly();
    }

    public static void GradAndHess(
        this Expr expr,
        out ReadOnlyCollectionOfExpressions gradient,
        out ReadOnlyCollectionOfReadOnlyCollectionOfExpressions hessian)
    {
        var vars = expr.GetVariables();
        var grad = new List<Expr>();
        var hess = new List<ReadOnlyCollection<Expr>>();
        foreach (var var1 in vars)
        {
            var grad_i = expr.Derivative(var1);
            grad.Add(grad_i);
            var hessRow = new List<Expr>();
            foreach (var var2 in vars)
            {
                var hess_ij = grad_i.Derivative(var2);
                hessRow.Add(hess_ij);
            }

            hess.Add(hessRow.AsReadOnly());
        }

        gradient = grad.AsReadOnly();
        hessian = hess.AsReadOnly();
    }

    public static Expr Sum(this IEnumerable<Expr> enumerable)
    {
        return enumerable.Aggregate((expr, expr1) => expr + expr1);
    }
}