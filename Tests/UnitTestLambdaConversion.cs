using System.Collections.ObjectModel;
using GoodSimonVM.AutoDiffLib.Builders;
using GoodSimonVM.AutoDiffLib.Expressions;
using GoodSimonVM.AutoDiffLib.LambdaConversion;
using ADMath = GoodSimonVM.AutoDiffLib.Expressions.Math;
using Math = System.Math;

namespace Tests;

public class UnitTestLambdaConversion
{
    private static IEnumerable<object[]> GetTestData(int countTests, int seed = 0)
    {
        var rnd = new Random(seed);
        List<object[]> data = new();
        var type = typeof(ParameterConversionRule);
        while (countTests-- > 0)
        {
            var a = rnd.NextDouble();
            var b = rnd.NextDouble();
            var x = new[] {rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()};

            data.AddRange(Enum.GetNames(type).Select(pcr => new object[] {a, b, x, Enum.Parse(type, pcr)}));
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(GetTestData), parameters: new object[] {1, 0})]
    public void Test(double a, double b, IReadOnlyList<double> x, ParameterConversionRule pcRule)
    {
        var expected = ExpectedFunction(a, b, x);
        var actualExpr = ActualExpr(a, b, x);

        var actualLambda = actualExpr.ConvertToLambda(
            new ConvertOptions {ParameterRule = pcRule});
        var actualDelegate = actualLambda.Compile();
        double actual;

        switch (pcRule)
        {
            case ParameterConversionRule.NotGroup:
            {
                var actualFunc = (Func<double, double, double, double, double>) actualDelegate;
                actual = actualFunc(x[0], x[1], x[2], x[3])!;
                break;
            }
            case ParameterConversionRule.GroupInArrayByName:
            {
                var actualFunc = (Func<double[], double>) actualDelegate;
                actual = actualFunc(x.ToArray());
                break;
            }
            case ParameterConversionRule.GroupAll:
            {
                var actualFunc = (Func<double[], double>) actualDelegate;
                actual = actualFunc(x.ToArray());
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(pcRule), pcRule, null);
        }

        Assert.Equal(expected, actual);
    }

    private static Expr ActualExpr(double a, double b, IReadOnlyList<double> x)
    {
        var actualExpr = ExpressionBuilder.Create(
            "x", x.Count,
            new List<ConstantExpr> {a, b}.AsReadOnly(),
            (constants,
                    variables)
                =>
            {
                Expr sum = 0d;
                var a = constants[0]!;
                var b = constants[1]!;

                for (var i = 0; i < variables.Count - 1; i++)
                {
                    var xi = variables[i];
                    var xj = variables[i + 1];
                    sum += (a - xi).Pow2() + b * (xj - xi.Pow2()).Pow2();
                }

                return sum;
            },
            out _);
        return actualExpr;
    }

    private static Expr ActualExprBuilder(
        ReadOnlyCollection<ConstantExpr> constants,
        ReadOnlyCollection<VariableExpr> variables)
    {
        Expr sum = 0d;
        var a = constants[0]!;
        var b = constants[1]!;

        for (var i = 0; i < variables.Count - 1; i++)
        {
            var xi = variables[i];
            var xj = variables[i + 1];
            sum += (a - xi).Pow2() + b * (xj - xi.Pow2()).Pow2();
        }

        return sum;
    }


    private static double ExpectedFunction(double a, double b, IReadOnlyList<double> x)
    {
        var sum = 0d;
        for (var i = 0; i < x.Count - 1; i++)
        {
            var xi = x[i];
            var xj = x[i + 1];
            sum += Math.Pow(a - xi, 2) + b * Math.Pow(xj - Math.Pow(xi, 2), 2);
        }

        return sum;
    }

    [Fact]
    public static void TestTypeConversion()
    {
        var expr = ExpressionBuilder.Create(new[] {"x", "y"}, vars =>
        {
            var x = vars[0];
            var y = vars[1];
            return ADMath.Sgn(x) * y;
        }, out var vars);
        var lambda = expr.ConvertToLambda(new ConvertOptions
            {ParameterRule = ParameterConversionRule.GroupInArrayByName});
        var @delegate = lambda.Compile();
        return;
    }
}