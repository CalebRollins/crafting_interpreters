class RPN : Expr.Visitor<string>
{
	public string print(Expr expr) => expr.accept(this);

	public string visitAssignExpr(Expr.Assign expr)
	{
		throw new NotImplementedException();
	}

	public string visitBinaryExpr(Expr.Binary expr) =>
		$"{print(expr.left)} {print(expr.right)} {expr.op.lexeme}";

	public string visitGroupingExpr(Expr.Grouping expr) =>
		print(expr.expression);

	public string visitLiteralExpr(Expr.Literal expr) =>
		expr.value == null ? "nil" : expr.value.ToString()!;

	public string visitLogicalExpr(Expr.Logical expr)
	{
		throw new NotImplementedException();
	}

	public string visitUnaryExpr(Expr.Unary expr) =>
		$"{print(expr.right)}{expr.op.lexeme}";

	public string visitVariableExpr(Expr.Variable expr)
	{
		throw new NotImplementedException();
	}
}