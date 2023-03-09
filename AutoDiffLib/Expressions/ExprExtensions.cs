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
        var derivatives = new List<Expr>();
        foreach (var var in vars)
        {
            var derivative = expr.Derivative(var);
            derivatives.Add(derivative);
        }

        return derivatives.AsReadOnly();
    }

    public static ReadOnlyCollectionOfReadOnlyCollectionOfExpressions Hess(this Expr expr)
    {
        var vars = expr.GetVariables();
        var hessian = new List<ReadOnlyCollection<Expr>>();
        foreach (var var1 in vars)
        {
            var d1 = expr.Derivative(var1);
            var d2s = new List<Expr>();
            foreach (var var2 in vars)
            {
                var d2 = d1.Derivative(var2);
                d2s.Add(d2);
            }

            hessian.Add(d2s.AsReadOnly());
        }

        return hessian.AsReadOnly();
    }
}