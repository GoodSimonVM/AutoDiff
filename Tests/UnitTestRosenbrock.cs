using GoodSimonVM.AutoDiffLib.Builders;
using GoodSimonVM.AutoDiffLib.Expressions;
using ADMath = GoodSimonVM.AutoDiffLib.Expressions.Math;
using Math = System.Math;

namespace Tests;

public class UnitTestRosenbrock
{
    public static IEnumerable<object[]> RosenbrockData => new List<object[]>
    {
        new object[] { 1, 100, new double[] { 1, 1 } },
        new object[] { 1, 100, new double[] { 1, 1, 1 } },
        new object[] { 1, 0.1, new double[] { 10, 10 } },
        new object[] { 1, 0.1, new double[] { 10, 10, 10 } },
        new object[] { 1, 0.1, new double[] { 10, 10, 10, 10 } },
        new object[] { 1, 0.1, new double[] { 10, 10, 10, 10, 10 } },
        new object[] { 1, 0.1, new double[] { 1456, 453 } },
        new object[] { 1, 0.1, new double[] { 1456, 453, 555 } },
        new object[] { 1, 0.1, new double[] { 111, 222 } },
        new object[] { 1, 0.1, new double[] { 111, 222, 333 } },
        new object[] { 1, 0.1, new double[] { 111, 222, 333, 444 } },
        new object[] { 1, 0.1, new double[] { 111, 222, 333, 444, 555 } },
    };

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockFunction(double a, double b, double[] x)
    {
        var functionResult = RosenbrockFunction(a, b, x);
        var expressionResult = RosenbrockExprEval(a, b, x);
        Assert.Equal(functionResult, expressionResult);
    }

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockGradient(double a, double b, double[] x)
    {
        var functionResult = RosenbrockGradient(a, b, x);
        var expressionResult = RosenbrockGradientExprEval(a, b, x);
        
        Assert.Equal(functionResult, expressionResult);
    }

    [Theory]
    [MemberData(nameof(RosenbrockData))]
    public void TestRosenbrockHessian(double a, double b, double[] x)
    {
        var functionResult = RosenbrockHess(a, b, x);
        var expressionResult = RosenbrockHessianExprEval(a, b, x);
        Assert.Equal(functionResult, expressionResult);
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
                var dxidxk = dx(i, k);
                var dxjdxk = dx(j, k);
                var dfa = -2 * (a - xi) * dxidxk;
                var dfb = b * (2  * (xj - Math.Pow(xi,2)) * (dxjdxk - 2 * xi * dxidxk));
                g += dfa + dfb;
            }

            grad[k] = g;
        }

        int dx(int i, int k)
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

    private static Expr RosenbrockExprBuilder(
        IReadOnlyList<ConstantExpr> constants,
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

    private static double RosenbrockExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = ExpressionBuilder.Create(
            "x", x.Count,
            new ConstantExpr[] { a, b },
            RosenbrockExprBuilder,
            out var variables);
        for (var i = 0; i < variables.Count; i++)
            variables[i].Value = x[i];
        var result = rosenbrockExpr.Evaluate();

        return result;
    }

    private static double[] RosenbrockGradientExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = ExpressionBuilder.Create(
            "x", x.Count,
            new ConstantExpr[] { a, b },
            RosenbrockExprBuilder,
            out var variables);
        for (var i = 0; i < variables.Count; i++)
            variables[i].Value = x[i];
        var gradExprs = rosenbrockExpr.Grad();
        var grad = gradExprs.Select(e => e.Evaluate()).ToArray();
        return grad;
    }

    private static double[,] RosenbrockHessianExprEval(
        double a,
        double b,
        IReadOnlyList<double> x)
    {
        var rosenbrockExpr = ExpressionBuilder.Create(
            "x", x.Count,
            new ConstantExpr[] { a, b },
            RosenbrockExprBuilder,
            out var variables);
        for (var i = 0; i < variables.Count; i++)
            variables[i].Value = x[i];
        var hessExprs = rosenbrockExpr.Hess();
        var hess = new double[x.Count, x.Count];

        for (var i = 0; i < hessExprs.Count; i++)
        for (var j = 0; j < hessExprs.Count; j++)
            hess[i, j] = hessExprs[i][j].Evaluate();

        return hess;
    }
}