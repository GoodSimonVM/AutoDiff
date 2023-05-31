using System.Collections.Generic;
using System.Linq;
using GoodSimonVM.AutoDiffLib.Exceptions;

namespace GoodSimonVM.AutoDiffLib.Utils;

internal static class StringUtils
{
    public const string ListSeparator = ", ";
    public const string ListSeparatorFormat = ListSeparator + "{0}";

    public static char ToSuperscript(char ch)
    {
        ch = ch switch
        {
            'i' => '\u2071',
            '0' => '\u2070',
            '1' => '\u00b9',
            '2' => '\u00b2',
            '3' => '\u00b3',
            '4' => '\u2074',
            '5' => '\u2075',
            '6' => '\u2076',
            '7' => '\u2077',
            '8' => '\u2078',
            '9' => '\u2079',
            '+' => '\u207a',
            '-' => '\u207b',
            '=' => '\u207c',
            '(' => '\u207d',
            ')' => '\u207e',
            'n' => '\u207f',
            _ => throw new CharNotSupportedException(ch)
        };
        return ch;
    }

    public static char ToSubscript(char ch)
    {
        ch = ch switch
        {
            '0' => '\u2080',
            '1' => '\u2081',
            '2' => '\u2082',
            '3' => '\u2083',
            '4' => '\u2084',
            '5' => '\u2085',
            '6' => '\u2086',
            '7' => '\u2087',
            '8' => '\u2088',
            '9' => '\u2089',
            '+' => '\u208a',
            '-' => '\u208b',
            '=' => '\u208c',
            '(' => '\u208d',
            ')' => '\u208e',
            'a' => '\u2090',
            'e' => '\u2091',
            'o' => '\u2092',
            'x' => '\u2093',
            'ə' => '\u2094',
            'h' => '\u2095',
            'k' => '\u2096',
            'l' => '\u2097',
            'm' => '\u2098',
            'n' => '\u2099',
            'p' => '\u209a',
            's' => '\u209b',
            't' => '\u209c',
            _ => throw new CharNotSupportedException(ch)
        };
        return ch;
    }

    public static char ToNormal(char ch)
    {
        ch = ch switch
        {
            '\u2070' => '0',
            '\u2080' => '0',
            '\u00b9' => '1',
            '\u2081' => '1',
            '\u00b2' => '2',
            '\u2082' => '2',
            '\u00b3' => '3',
            '\u2083' => '3',
            '\u2074' => '4',
            '\u2084' => '4',
            '\u2075' => '5',
            '\u2085' => '5',
            '\u2076' => '6',
            '\u2086' => '6',
            '\u2077' => '7',
            '\u2087' => '7',
            '\u2078' => '8',
            '\u2088' => '8',
            '\u2079' => '9',
            '\u2089' => '9',
            '\u207a' => '+',
            '\u208a' => '+',
            '\u207b' => '-',
            '\u208b' => '-',
            '\u207c' => '=',
            '\u208c' => '=',
            '\u207d' => '(',
            '\u208d' => '(',
            '\u207e' => ')',
            '\u208e' => ')',
            '\u2090' => 'a',
            '\u2091' => 'e',
            '\u2092' => 'o',
            '\u2093' => 'x',
            '\u2094' => 'ə',
            '\u2095' => 'h',
            '\u2096' => 'k',
            '\u2097' => 'l',
            '\u2098' => 'm',
            '\u207f' => 'n',
            '\u2099' => 'n',
            '\u209a' => 'p',
            '\u209b' => 's',
            '\u209c' => 't',
            '\u2071' => 'i',
            _ => throw new CharNotSupportedException(ch)
        };
        return ch;
    }

    public static readonly IEnumerable<char> SubscriptChars = new[]
    {
        '\u2080',
        '\u2081',
        '\u2082',
        '\u2083',
        '\u2084',
        '\u2085',
        '\u2086',
        '\u2087',
        '\u2088',
        '\u2089',
        '\u208a',
        '\u208b',
        '\u208c',
        '\u208d',
        '\u208e',
        '\u2090',
        '\u2091',
        '\u2092',
        '\u2093',
        '\u2094',
        '\u2095',
        '\u2096',
        '\u2097',
        '\u2098',
        '\u2099',
        '\u209a',
        '\u209b',
        '\u209c',
    };

    public static readonly IEnumerable<char> SuperscriptChars = new[]
    {
        '\u2071',
        '\u2070',
        '\u00b9',
        '\u00b2',
        '\u00b3',
        '\u2074',
        '\u2075',
        '\u2076',
        '\u2077',
        '\u2078',
        '\u2079',
        '\u207a',
        '\u207b',
        '\u207c',
        '\u207d',
        '\u207e',
        '\u207f',
    };

    public static string ToSuperscript(string str)
    {
        return new string(str.Select(ToSuperscript).ToArray());
    }

    public static string ToSubscript(string str)
    {
        return new string(str.Select(ToSubscript).ToArray());
    }

    public static string ToNormal(string str)
    {
        return new string(str.Select(ToNormal).ToArray());
    }

    public static string MakeIndexedName(string commonName, int index)
    {
        return $"{commonName}{StringUtils.ToSubscript(index.ToString())}";
    }

    public static bool TryParseIndexedName(string name, out string commonName, out int index)
    {
        var firstSubscriptIndex = name.IndexOfAny(StringUtils.SubscriptChars.ToArray());
        if (firstSubscriptIndex < 0)
        {
            commonName = null!;
            index = 0;
            return false;
        }

        commonName = name.Substring(0, firstSubscriptIndex);
        var indexStr = name.Substring(firstSubscriptIndex, name.Length - firstSubscriptIndex);
        indexStr = ToNormal(indexStr);
        return int.TryParse(indexStr, out index);
    }
}