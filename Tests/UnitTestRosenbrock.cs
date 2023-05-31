using GoodSimonVM.AutoDiffLib.Builders;
using GoodSimonVM.AutoDiffLib.Expressions;
using ADMath = GoodSimonVM.AutoDiffLib.Expressions.Math;
using Math = System.Math;

namespace Tests;

public class UnitTestRosenbrock
{
    public static IEnumerable<object[]> RosenbrockData => new List<object[]>
    {
        new object[] {1, 100, new double[] {1, 1}},
        new object[] {1, 100, new double[] {1, 1, 1}},
        new object[] {1, 0.1, new double[] {10, 10}},
        new object[] {1, 0.1, new double[] {10, 10, 10}},
        new object[] {1, 0.1, new double[] {10, 10, 10, 10}},
        new object[] {1, 0.1, new double[] {10, 10, 10, 10, 10}},
        new object[] {1, 0.1, new double[] {1456, 453}},
        new object[] {1, 0.1, new double[] {1456, 453, 555}},
        new object[] {1, 0.1, new double[] {11.1, 22.2}},
        new object[] {1, 0.1, new double[] {11.1, 22.2, 33.3}},
        new object[] {1, 0.1, new double[] {11.1, 22.2, 33.3, 44.4}},
        new object[] {1, 0.1, new double[] {11.1, 22.2, 33.3, 44.4, 55.5}},
    };

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockFunction(double a, double b, double[] x)
    {
        var expected = RosenbrockFunction(a, b, x);
        var actual = RosenbrockExprEval(a, b, x);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockGradient(double a, double b, double[] x)
    {
        var expected = RosenbrockGradient(a, b, x);
        var actual = RosenbrockGradientExprEval(a, b, x);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockHessian(double a, double b, double[] x)
    {
        var expected = RosenbrockHess(a, b, x);
        var actual = RosenbrockHessianExprEval(a, b, x);
        Assert.Equal(expected, actual);
    }

    private static double RosenbrockFunction(
        double a,
        double b,
        IReadOnlyList<double> x)
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

    private static double[] RosenbrockGradient(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var grad = new double[x.Count];
        for (var k = 0; k < x.Count; k++)
        {
            var g = 0d;
            for (var i = 0; i < x.Count - 1; i++)
            {
                var j = i + 1;
                var xi = x[i];
                var xj = x[j];
                var dxidxk = xdx(i, k);
                var dxjdxk = xdx(j, k);
                var dfa = -2 * (a - xi) * dxidxk;
                var dfb = b * (2 * (xj - Math.Pow(xi, 2)) * (dxjdxk - 2 * xi * dxidxk));
                g += dfa + dfb;
            }

            grad[k] = g;
        }

        int xdx(int i, int k)
        {
            return i == k ? 1 : 0;
        }

        return grad;
    }

    private static double[,] RosenbrockHess(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var hess = new double[x.Count, x.Count];
        for (var k = 0; k < x.Count; k++)
        for (var l = 0; l < x.Count; l++)
        {
            var h = 0d;
            for (var i = 0; i < x.Count - 1; i++)
            {
                var j = i + 1;
                var xi = x[i];
                var xj = x[j];
                var dxidxk = dx(i, k);
                var dxjdxk = dx(j, k);
                var dxidxl = dx(i, l);
                var dxjdxl = dx(j, l);

                h +=
                    2 * dxidxk * dxidxl +
                    2 * b *
                    ((dxjdxl - 2 * xi * dxidxl) * (dxjdxk - 2 * xi * dxidxk) +
                     (xj - xi * xi) * -2 * dxidxl * dxidxk);
            }

            hess[k, l] = h;
        }

        int dx(int i, int di)
        {
            return i == di ? 1 : 0;
        }

        return hess;
    }

    private static Expr GetRosenbrockExpr(
        double a,
        double b,
        IReadOnlyList<double> x,
        out IDictionary<string, double> values)
    {
        var xVars = ExpressionBuilder.BuildVariables("x", x.Count);
        var aVar = ExpressionBuilder.BuildVariable("a");
        var bVar = ExpressionBuilder.BuildVariable("b");

        var vars = xVars.Concat(new[] {aVar, bVar}).ToList().AsReadOnly();

        var rosenbrockExpr = ExpressionBuilder.Create(
            vars,
            (vars) =>
            {
                var variables = new List<VariableExpr>();
                var constants = new List<VariableExpr>();
                foreach (var x in vars)
                {
                    if (x.Name.StartsWith("x"))
                        variables.Add(x);
                    else
                        constants.Add(x);
                }

                return RosenbrockExprBuilder(constants, variables);
            });

        values = PrepareVariableValues(vars, x.Concat(new[] {a, b}).ToList());

        return rosenbrockExpr;
    }

    private static Expr RosenbrockExprBuilder(
        IReadOnlyList<VariableExpr> constants,
        IReadOnlyList<VariableExpr> variables)
    {
        Expr sum = 0d;
        var a = constants[0];
        var b = constants[1];

        for (var i = 0; i < variables.Count - 1; i++)
        {
            var xi = variables[i];
            var xj = variables[i + 1];
            sum += (a - xi).Pow2() + b * (xj - xi.Pow2()).Pow2();
        }

        return sum;
    }

    private static Dictionary<string, double> PrepareVariableValues(
        IReadOnlyList<VariableExpr> variables,
        IReadOnlyList<double> x)
    {
        var values = variables
            .Zip(x, (variable, value) => new {variable.Name, Value = value})
            .ToDictionary(kvp => kvp.Name, kvp => kvp.Value);
        return values;
    }

    private static double RosenbrockExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = GetRosenbrockExpr(a, b, x, out var values);

        var result = rosenbrockExpr.Evaluate(values);

        return result;
    }

    private static double[] RosenbrockGradientExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = GetRosenbrockExpr(a, b, x, out var values);
        var xVars = rosenbrockExpr.GetVariables().Where(v => v.Name.StartsWith("x")).ToList();

        var gradExprs = rosenbrockExpr.Grad(wrts: xVars);
        var grad = gradExprs.Select(e => e.Evaluate(values)).ToArray();
        return grad;
    }

    private static double[,] RosenbrockHessianExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = GetRosenbrockExpr(a, b, x, out var values);
        var xVars = rosenbrockExpr.GetVariables().Where(v => v.Name.StartsWith("x")).ToList();
        var hessExprs = rosenbrockExpr.Hess(wrts: xVars);
        var hess = new double[x.Count, x.Count];

        for (var i = 0; i < hessExprs.Count; i++)
        for (var j = 0; j < hessExprs.Count; j++)
            hess[i, j] = hessExprs[i][j].Evaluate(values);

        return hess;
    }
}