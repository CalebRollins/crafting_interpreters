abstract class Expr
{
	public abstract R accept<R>(Visitor<R> visitor);

	public interface Visitor<R>
	{
		R visitBinaryExpr(Binary expr);
		R visitGroupingExpr(Grouping expr);
		R visitLiteralExpr(Literal expr);
		R visitUnaryExpr(Unary expr);
	}

	public class Binary : Expr
	{
		public readonly Expr left;
		public readonly Token op;
		public readonly Expr right;

		Binary(Expr left, Token op, Expr right)
		{
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override R accept<R>(Visitor<R> visitor) =>
			visitor.visitBinaryExpr(this);
	}

	public class Grouping : Expr
	{
		public readonly Expr expression;

		Grouping(Expr expression)
		{
			this.expression = expression;
		}

		public override R accept<R>(Visitor<R> visitor) =>
			visitor.visitGroupingExpr(this);
	}

	public class Literal : Expr
	{
		public readonly object value;

		Literal(object value)
		{
			this.value = value;
		}

		public override R accept<R>(Visitor<R> visitor) =>
			visitor.visitLiteralExpr(this);
	}

	public class Unary : Expr
	{
		public readonly Token op;
		public readonly Expr right;

		Unary(Token op, Expr right)
		{
			this.op = op;
			this.right = right;
		}

		public override R accept<R>(Visitor<R> visitor) =>
			visitor.visitUnaryExpr(this);
	}

}
