using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GoodSimonVM.AutoDiffLib.Expressions;
using Math = GoodSimonVM.AutoDiffLib.Expressions.Math;

namespace GoodSimonVM.AutoDiffLib.LambdaConversion;

/*
public enum ConstantConversionRule
{
    ConvertToParameter,
    UseSetValue,
}

public enum ParameterConversionRule
{
    NotGroup,
    GroupInArrayByName,
    GroupAll,
}

public struct ConvertOptions
{
    public ParameterConversionRule ParameterRule { get; set; }
    public ConstantConversionRule ConstantRule { get; set; }
}*/

public static class LambdaConversionExtensions
{
    public static LambdaExpression ConvertToLambda(this Expr expr)
    {
        var parameters = MakeParameters(expr.GetVariables());
        var body = ConvertBody(expr, parameters);
        return Expression.Lambda(body, parameters);
    }

    private static ParameterExpression[] MakeParameters(IEnumerable<VariableExpr> variables)
    {
        return variables.Select(variable => Expression.Parameter(typeof(double), variable.Name)).ToArray();
    }

    private static Expression ConvertBody(this Expr expr, IEnumerable<ParameterExpression> parameterExpressions)
    {
        switch (expr)
        {
            case AddExpr addExpr:
            {
                var l = addExpr.Left.ConvertBody(parameterExpressions);
                var r = addExpr.Right.ConvertBody(parameterExpressions);
                return Expression.Add(l, r);
            }
            case DivExpr divExpr:
            {
                var l = divExpr.Left.ConvertBody(parameterExpressions);
                var r = divExpr.Right.ConvertBody(parameterExpressions);
                return Expression.Divide(l, r);
            }
            case LgExpr lgExpr:
            {
                Func<double, double> m = System.Math.Log10;
                var mi = m.Method;
                var xExpression = lgExpr.X.ConvertBody(parameterExpressions);
                return Expression.Call(mi, xExpression);
            }
            case LnExpr lnExpr:
            {
                Func<double, double> m = System.Math.Log;
                var mi = m.Method;
                var xExpression = lnExpr.X.ConvertBody(parameterExpressions);
                return Expression.Call(mi, xExpression);
            }
            case LogExpr logExpr:
            {
                Func<double, double, double> m = System.Math.Log;
                var mi = m.Method;
                var xExpression = logExpr.X.ConvertBody(parameterExpressions);
                var baseExpression = logExpr.Base.ConvertBody(parameterExpressions);
                return Expression.Call(mi, xExpression, baseExpression);
            }
            case MinusExpr minusExpr:
            {
                var ne = minusExpr.Right.ConvertBody(parameterExpressions);
                return Expression.Negate(ne);
            }
            case MulExpr mulExpr:
            {
                var l = mulExpr.Left.ConvertBody(parameterExpressions);
                var r = mulExpr.Right.ConvertBody(parameterExpressions);
                return Expression.Multiply(l, r);
            }
            case PowerExpr powerExpr:
            {
                Func<double, double, double> m = System.Math.Pow;
                var mi = m.Method;
                var baseExpression = powerExpr.Base.ConvertBody(parameterExpressions);
                var powerExpression = powerExpr.Power.ConvertBody(parameterExpressions);
                return Expression.Call(mi, baseExpression, powerExpression);
            }
            case SubExpr subExpr:
            {
                var l = subExpr.Left.ConvertBody(parameterExpressions);
                var r = subExpr.Right.ConvertBody(parameterExpressions);
                return Expression.Subtract(l, r);
            }
            case AbsExpr absExpr:
            {
                Func<double, double> m = System.Math.Abs;
                var mi = m.Method;
                var argumentExpression = absExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Call(mi, argumentExpression);
            }
            case ConstantExpr constantExpr:
            {
                return Expression.Constant(constantExpr.Value);
            }
            case CosecantExpr cosecantExpr:
            {
                Func<double, double> m = System.Math.Sin;
                var mi = m.Method;
                var expressionArgument = cosecantExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
            }
            case CosExpr cosExpr:
            {
                Func<double, double> m = System.Math.Cos;
                var mi = m.Method;
                var expressionArgument = cosExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Call(mi, expressionArgument);
            }
            case CotExpr cotExpr:
            {
                Func<double, double> m = System.Math.Tan;
                var mi = m.Method;
                var expressionArgument = cotExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
            }
            case PlusExpr plusExpr:
            {
                return Expression.UnaryPlus(plusExpr.Expr.ConvertBody(parameterExpressions));
            }
            case SecantExpr secantExpr:
            {
                Func<double, double> m = System.Math.Cos;
                var mi = m.Method;
                var expressionArgument = secantExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
            }
            case SignExpr signExpr:
            {
                Func<double, int> m = System.Math.Sign;
                var mi = m.Method;
                var xExpression = signExpr.Expr.ConvertBody(parameterExpressions);
                xExpression = Expression.Convert(xExpression, typeof(double));
                return Expression.Call(mi, xExpression);
            }
            case SinExpr sinExpr:
            {
                Func<double, double> m = System.Math.Sin;
                var mi = m.Method;
                var expressionArgument = sinExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Call(mi, expressionArgument);
            }
            case TanExpr tanExpr:
            {
                Func<double, double> m = System.Math.Tan;
                var mi = m.Method;
                var expressionArgument = tanExpr.Expr.ConvertBody(parameterExpressions);
                return Expression.Call(mi, expressionArgument);
            }
            case VariableExpr variableExpr:
            {
                var variableExprName = variableExpr.Name;
                return parameterExpressions.Single(parameter =>
                    parameter.Name.Equals(variableExprName, StringComparison.InvariantCulture));
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(expr));
        }
    }

    private static MethodInfo GetMathMethodInfoByName(string methodName)
    {
        return typeof(Math)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(mi =>
                mi.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase) &&
                mi.ReturnType == typeof(double));
    }

    private static ParameterExpression Convert(this VariableExpr expr)
    {
        return Expression.Parameter(typeof(double), expr.Name);
    }

    private static ConstantExpression Convert(this ConstantExpr expr)
    {
        return Expression.Constant(expr.Value);
    }

    private static ExpressionType GetType(this Expr expr)
    {
        switch (expr)
        {
            case ConstantExpr:
                return ExpressionType.Parameter;
            case VariableExpr:
                return ExpressionType.Parameter;

            case MinusExpr:
                return ExpressionType.Negate;
            case PlusExpr:
                return ExpressionType.UnaryPlus;

            case AddExpr:
                return ExpressionType.Add;
            case SubExpr:
                return ExpressionType.Subtract;

            case DivExpr:
                return ExpressionType.Divide;
            case MulExpr:
                return ExpressionType.Multiply;

            case LgExpr:
            case LnExpr:
            case LogExpr:
            case PowerExpr:
            case AbsExpr:
            case CosecantExpr:
            case CosExpr:
            case CotExpr:
            case SecantExpr:
            case SignExpr:
            case SinExpr:
            case TanExpr:
                return ExpressionType.Call;
            default:
                throw new ArgumentOutOfRangeException(nameof(expr));
        }
    }
}