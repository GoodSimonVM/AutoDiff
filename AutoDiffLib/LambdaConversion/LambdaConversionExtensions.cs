﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GoodSimonVM.AutoDiffLib.Expressions;
using GoodSimonVM.AutoDiffLib.Utils;
using ADMath = GoodSimonVM.AutoDiffLib.Expressions.Math;
using Math = System.Math;

#if NET5_0_OR_GREATER
using ReadOnlyCollectionOfVariableParameterInfos =
 System.Collections.Generic.IReadOnlyList<GoodSimonVM.AutoDiffLib.LambdaConversion.VariableParameterInfo>;
#else
using ReadOnlyCollectionOfVariableParameterInfos =
    System.Collections.ObjectModel.ReadOnlyCollection<GoodSimonVM.AutoDiffLib.LambdaConversion.VariableParameterInfo>;
#endif

namespace GoodSimonVM.AutoDiffLib.LambdaConversion;

public static class LambdaConversionExtensions
{
    public static LambdaExpression ConvertToLambda(this Expr expr, ConvertOptions options = default)
    {
        var variableParameterInfos = MakeParameters(expr.GetVariables().ToArray(), options);
        var body = ConvertBody(expr, variableParameterInfos, options);
        return Expression.Lambda(body, variableParameterInfos.Select(v => v.CommonParameter).Distinct());
    }

    private static ReadOnlyCollectionOfVariableParameterInfos MakeParameters(
        IList<VariableExpr> variables,
        ConvertOptions options)
    {
        List<VariableParameterInfo> accessExpressions = new();

        switch (options.ParameterRule)
        {
            case ParameterConversionRule.GroupAll:
            {
                var commonParameter = Expression.Parameter(typeof(double[]), "variables");
                for (var index = 0; index < variables.Count; index++)
                {
                    var variable = variables[index];
                    var accessExpression = Expression.ArrayIndex(commonParameter, Expression.Constant(index));
                    accessExpressions.Add(new VariableParameterInfo()
                    {
                        Name = variable.Name,
                        CommonParameter = commonParameter,
                        AccessExpression = accessExpression,
                    });
                }

                break;
            }
            case ParameterConversionRule.GroupInArrayByName:
            {
                accessExpressions = variables
                    .Select(var =>
                    {
                        StringUtils.TryParseIndexedName(var.Name, out var commonName, out var index);
                        return new
                        {
                            var.Name, CommonName = commonName, Index = index
                        };
                    })
                    .GroupBy(
                        tmp => tmp.CommonName,
                        tmp => new { tmp.Name, tmp.Index },
                        (commonName, tmp) =>
                        {
                            var commonArrayParameter = Expression.Parameter(typeof(double[]), commonName);
                            return new
                            {
                                CommonArrayParameter = commonArrayParameter,
                                AccessExpressionPairs = tmp.Select(
                                    var =>
                                    {
                                        Expression accessExpr = var.Index >= 0
                                            ? Expression.ArrayIndex(
                                                array: commonArrayParameter,
                                                index: Expression.Constant(var.Index))
                                            : commonArrayParameter;
                                        return new KeyValuePair<string, Expression>(var.Name, accessExpr);
                                    })
                            };
                        })
                    .SelectMany(
                        t => t.AccessExpressionPairs,
                        (t, kvp) => new VariableParameterInfo
                        {
                            Name = kvp.Key,
                            CommonParameter = t.CommonArrayParameter,
                            AccessExpression = kvp.Value
                        })
                    .ToList();
                break;
            }
            case ParameterConversionRule.NotGroup:
            {
                accessExpressions = variables.Select(variable =>
                {
                    var parameter = Expression.Parameter(typeof(double), variable.Name);
                    return new VariableParameterInfo
                    {
                        Name = variable.Name,
                        CommonParameter = parameter,
                        AccessExpression = parameter
                    };
                }).ToList();
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }


        return accessExpressions.AsReadOnly();
    }

    private static Expression ConvertBody(
        this Expr expr,
        ReadOnlyCollectionOfVariableParameterInfos variableParameterInfos,
        ConvertOptions options
    )
    {
        Expression resultExpression;
        switch (expr)
        {
            case AddExpr addExpr:
            {
                var l = addExpr.Left.ConvertBody(variableParameterInfos, options);
                var r = addExpr.Right.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Add(l, r);
                break;
            }
            case DivExpr divExpr:
            {
                var l = divExpr.Left.ConvertBody(variableParameterInfos, options);
                var r = divExpr.Right.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Divide(l, r);
                break;
            }
            case LgExpr lgExpr:
            {
                Func<double, double> m = System.Math.Log10;
                var mi = m.Method;
                var xExpression = lgExpr.X.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, xExpression);
                break;
            }
            case LnExpr lnExpr:
            {
                Func<double, double> m = System.Math.Log;
                var mi = m.Method;
                var xExpression = lnExpr.X.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, xExpression);
                break;
            }
            case LogExpr logExpr:
            {
                Func<double, double, double> m = System.Math.Log;
                var mi = m.Method;
                var xExpression = logExpr.X.ConvertBody(variableParameterInfos, options);
                var baseExpression =
                    logExpr.Base.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, xExpression, baseExpression);
                break;
            }
            case MinusExpr minusExpr:
            {
                var ne = minusExpr.Right.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Negate(ne);
                break;
            }
            case MulExpr mulExpr:
            {
                var re = mulExpr.Right;
                var le = mulExpr.Left;
                if (le is ConstantExpr cle && Math.Abs(cle.Value - (-1)) < double.Epsilon)
                {
                    var r = re.ConvertBody(variableParameterInfos, options);
                    resultExpression = Expression.Negate(r);
                }
                else if (re is ConstantExpr rle && Math.Abs(rle.Value - (-1)) < double.Epsilon)
                {
                    var l = le.ConvertBody(variableParameterInfos, options);
                    resultExpression = Expression.Negate(l);
                }
                else
                {
                    var l = le.ConvertBody(variableParameterInfos, options);
                    var r = re.ConvertBody(variableParameterInfos, options);
                    resultExpression = Expression.Multiply(l, r);
                }

                break;
            }
            case PowerExpr powerExpr:
            {
                Func<double, double, double> m = System.Math.Pow;
                var mi = m.Method;
                var baseExpression =
                    powerExpr.Base.ConvertBody(variableParameterInfos, options);
                var powerExpression =
                    powerExpr.Power.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, baseExpression, powerExpression);
                break;
            }
            case SubExpr subExpr:
            {
                var l = subExpr.Left.ConvertBody(variableParameterInfos, options);
                var r = subExpr.Right.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Subtract(l, r);
                break;
            }
            case AbsExpr absExpr:
            {
                Func<double, double> m = System.Math.Abs;
                var mi = m.Method;
                var argumentExpression =
                    absExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, argumentExpression);
                break;
            }
            case ConstantExpr constantExpr:
            {
                resultExpression = Expression.Constant(constantExpr.Value);
                break;
            }
            case CosecantExpr cosecantExpr:
            {
                Func<double, double> m = System.Math.Sin;
                var mi = m.Method;
                var expressionArgument =
                    cosecantExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
                break;
            }
            case CosExpr cosExpr:
            {
                Func<double, double> m = System.Math.Cos;
                var mi = m.Method;
                var expressionArgument =
                    cosExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, expressionArgument);
                break;
            }
            case CotExpr cotExpr:
            {
                Func<double, double> m = System.Math.Tan;
                var mi = m.Method;
                var expressionArgument =
                    cotExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
                break;
            }
            case PlusExpr plusExpr:
            {
                var pExpr = plusExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.UnaryPlus(pExpr);
                break;
            }
            case SecantExpr secantExpr:
            {
                Func<double, double> m = System.Math.Cos;
                var mi = m.Method;
                var expressionArgument =
                    secantExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Divide(Expression.Constant(1), Expression.Call(mi, expressionArgument));
                break;
            }
            case SignExpr signExpr:
            {
                Func<double, int> m = System.Math.Sign;
                var mi = m.Method;
                var xExpression =
                    signExpr.Expr.ConvertBody(variableParameterInfos, options);
                xExpression = Expression.Convert(xExpression, typeof(double));
                resultExpression = Expression.Call(mi, xExpression);
                break;
            }
            case SinExpr sinExpr:
            {
                Func<double, double> m = System.Math.Sin;
                var mi = m.Method;
                var expressionArgument =
                    sinExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, expressionArgument);
                break;
            }
            case TanExpr tanExpr:
            {
                Func<double, double> m = System.Math.Tan;
                var mi = m.Method;
                var expressionArgument =
                    tanExpr.Expr.ConvertBody(variableParameterInfos, options);
                resultExpression = Expression.Call(mi, expressionArgument);
                break;
            }
            case VariableExpr variableExpr:
            {
                var variableExprName = variableExpr.Name;
                resultExpression = variableParameterInfos.Single(parameter =>
                    parameter.Name.Equals(variableExprName, StringComparison.InvariantCulture)).AccessExpression;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(expr));
        }

        if (resultExpression.Type != typeof(double))
            resultExpression = Expression.Convert(resultExpression, typeof(double));

        return resultExpression;
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