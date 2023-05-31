using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoodSimonVM.AutoDiffLib.Expressions;
using GoodSimonVM.AutoDiffLib.Utils;

namespace GoodSimonVM.AutoDiffLib.Exceptions;

public abstract class AutoDiffException : Exception
{
    protected AutoDiffException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

public class CharNotSupportedException : AutoDiffException
{
    private const string ExceptionMessage = "The char '{0}' is not supported";

    public CharNotSupportedException(char ch)
        : base(MakeMessage(ch), null)
    {
    }

    private static string MakeMessage(char ch)
    {
        return string.Format(ExceptionMessage, ch);
    }
}

public class NotUniqNamesException : AutoDiffException
{
    private const string ExceptionMessage =
        "Detected equal names: '{0}'. Set different name for one of variable, or use anouther constructor.";

    public NotUniqNamesException(IEnumerable<VariableExpr> vars)
        : base(MakeMessage(vars), null)
    {
    }

    private static string MakeMessage(IEnumerable<VariableExpr> vars)
    {
        var equalNames = vars
            .GroupBy(v => v.Name)
            .Select(g => g.Key)
            .Aggregate(new StringBuilder(), (sb, n) =>
                sb.Length > 0
                    ? sb.AppendFormat(StringUtils.ListSeparatorFormat, n)
                    : sb.Append(n))
            .ToString();
        return string.Format(ExceptionMessage, equalNames);
    }
}

public class ArgumentNotFound : AutoDiffException
{
    private const string ExceptionMessage =
        "The value of the variable '{0}' is not set.";

    public ArgumentNotFound(string variableName)
        : base(string.Format(ExceptionMessage, variableName), null)
    {
    }
}