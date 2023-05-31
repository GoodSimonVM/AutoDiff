using System;
using System.Collections.Generic;
using System.Linq;
using GoodSimonVM.AutoDiffLib.Exceptions;
using GoodSimonVM.AutoDiffLib.Expressions;
using GoodSimonVM.AutoDiffLib.Utils;

namespace GoodSimonVM.AutoDiffLib.Builders;

public static class ExpressionBuilder
{
    public static Expr Create(
        string commonVariableName,
        int count,
        Func<ReadOnlyCollectionOfVariables, Expr> func,
        out ReadOnlyCollectionOfVariables vars)
    {
        var names = BuildVariableNames(count, commonVariableName);
        return Create(names, func, out vars);
    }

    public static Expr Create(
        IEnumerable<string> varNames,
        Func<ReadOnlyCollectionOfVariables, Expr> func,
        out ReadOnlyCollectionOfVariables vars)
    {
        vars = BuildVariables(varNames);
        return Create(vars, func);
    }

    public static Expr Create(
        ReadOnlyCollectionOfVariables variables,
        Func<ReadOnlyCollectionOfVariables, Expr> func)
    {
        var countUniqNames = variables
            .Select(v => v.Name)
            .Distinct()
            .Count();
        if (countUniqNames != variables.Count)
            throw new NotUniqNamesException(variables);
        return func(variables.ToList().AsReadOnly());
    }

    public static Expr Create(
        string commonVariableName,
        int count,
        ReadOnlyCollectionOfConstants constants,
        Func<ReadOnlyCollectionOfConstants, ReadOnlyCollectionOfVariables, Expr> func,
        out ReadOnlyCollectionOfVariables vars)
    {
        var names = BuildVariableNames(count, commonVariableName);
        return Create(names, constants, func, out vars);
    }


    public static Expr Create(
        IEnumerable<string> varNames,
        ReadOnlyCollectionOfConstants constants,
        Func<ReadOnlyCollectionOfConstants, ReadOnlyCollectionOfVariables, Expr> func,
        out ReadOnlyCollectionOfVariables vars)
    {
        vars = BuildVariables(varNames);
        return Create(vars, constants, func);
    }

    public static Expr Create(
        ReadOnlyCollectionOfVariables variables,
        ReadOnlyCollectionOfConstants constants,
        Func<ReadOnlyCollectionOfConstants, ReadOnlyCollectionOfVariables, Expr> func)
    {
        var countUniqNames = variables
            .Select(v => v.Name)
            .Distinct()
            .Count();
        if (countUniqNames != variables.Count)
            throw new NotUniqNamesException(variables);
        return func(constants, variables.ToList().AsReadOnly());
    }

    private static IEnumerable<string> BuildVariableNames(int count, string commonName)
    {
        return Enumerable.Range(0, count)
            .Select(i => StringUtils.MakeIndexedName(commonName, i))
            .ToArray();
    }

    private static ReadOnlyCollectionOfVariables BuildVariables(IEnumerable<string> names)
    {
        return names
            .Select(BuildVariable)
            .ToList()
            .AsReadOnly();
    }

    public static ReadOnlyCollectionOfVariables BuildVariables(string commonName, int count)
    {
        return BuildVariables(BuildVariableNames(count, commonName));
    }

    public static VariableExpr BuildVariable(string name)
    {
        return new VariableExpr(name);
    }
}