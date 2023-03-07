using System.Linq.Expressions;

namespace GoodSimonVM.AutoDiffLib.LambdaConversion;
#nullable disable
internal class VariableParameterInfo
{
    public string Name { get; set; }
    public ParameterExpression CommonParameter { get; set; }
    public Expression AccessExpression { get; set; }
}