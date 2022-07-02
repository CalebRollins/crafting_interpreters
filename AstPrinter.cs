using System.Text;

class AstPrinter : Expr.Visitor<string>
{
	public string print(Expr expr) => expr.accept(this);

	public string visitBinaryExpr(Expr.Binary expr) =>
		parenthesize(expr.op.lexeme, expr.left, expr.right);

	public string visitGroupingExpr(Expr.Grouping expr) =>
		parenthesize("group", expr.expression);

	public string visitLiteralExpr(Expr.Literal expr) =>
		expr.value == null ? "nil" : expr.value.ToString()!;

	public string visitUnaryExpr(Expr.Unary expr) =>
		parenthesize(expr.op.lexeme, expr.right);

	private string parenthesize(string name, params Expr[] exprs)
	{
		var builder = new StringBuilder();

		builder.Append("(").Append(name);
		foreach (var expr in exprs)
		{
			builder.Append(" ");
			builder.Append(expr.accept(this));
		}
		builder.Append(")");

		return builder.ToString();
	}
}