abstract class Expr
{
	internal abstract R accept<R>(Visitor<R> visitor);

	internal interface Visitor<R>
	{
		R visitBinaryExpr(Binary expr);
		R visitGroupingExpr(Grouping expr);
		R visitLiteralExpr(Literal expr);
		R visitVariableExpr(Variable expr);
		R visitUnaryExpr(Unary expr);
	}

	internal class Binary : Expr
	{
		internal readonly Expr left;
		internal readonly Token op;
		internal readonly Expr right;

		internal Binary(Expr left, Token op, Expr right)
		{
			this.left = left;
			this.op = op;
			this.right = right;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitBinaryExpr(this);
	}

	internal class Grouping : Expr
	{
		internal readonly Expr expression;

		internal Grouping(Expr expression)
		{
			this.expression = expression;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitGroupingExpr(this);
	}

	internal class Literal : Expr
	{
		internal readonly object? value;

		internal Literal(object? value)
		{
			this.value = value;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitLiteralExpr(this);
	}

	internal class Variable : Expr
	{
		internal readonly Token name;

		internal Variable(Token name)
		{
			this.name = name;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitVariableExpr(this);
	}

	internal class Unary : Expr
	{
		internal readonly Token op;
		internal readonly Expr right;

		internal Unary(Token op, Expr right)
		{
			this.op = op;
			this.right = right;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitUnaryExpr(this);
	}

}
