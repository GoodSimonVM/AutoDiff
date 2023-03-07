using System.Linq.Expressions;

namespace GoodSimonVM.AutoDiffLib.LambdaConversion;

internal class VariableParameterInfo
{
    public string Name { get; set; }
    public ParameterExpression CommonParameter { get; set; }
    public Expression AccessExpression { get; set; }
}