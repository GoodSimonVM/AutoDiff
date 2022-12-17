#if NET5_0_OR_GREATER
global using ReadOnlyCollectionOfVariables = System.Collections.Generic.IReadOnlyList<GoodSimonVM.AutoDiffLib.Expressions.VariableExpr>;
global using ReadOnlyCollectionOfConstants = System.Collections.Generic.IReadOnlyList<GoodSimonVM.AutoDiffLib.Expressions.ConstantExpr>;
global using ReadOnlyCollectionOfExpressions = System.Collections.Generic.IReadOnlyList<GoodSimonVM.AutoDiffLib.Expressions.Expr>;
global using ReadOnlyCollectionOfReadOnlyCollectionOfExpressions = System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyList<GoodSimonVM.AutoDiffLib.Expressions.Expr>>;
#else
global using ReadOnlyCollectionOfVariables = System.Collections.ObjectModel.ReadOnlyCollection<GoodSimonVM.AutoDiffLib.Expressions.VariableExpr>;
global using ReadOnlyCollectionOfConstants = System.Collections.ObjectModel.ReadOnlyCollection<GoodSimonVM.AutoDiffLib.Expressions.ConstantExpr>;
global using ReadOnlyCollectionOfExpressions = System.Collections.ObjectModel.ReadOnlyCollection<GoodSimonVM.AutoDiffLib.Expressions.Expr>;
global using ReadOnlyCollectionOfReadOnlyCollectionOfExpressions = System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.ObjectModel.ReadOnlyCollection<GoodSimonVM.AutoDiffLib.Expressions.Expr>>;
#endif