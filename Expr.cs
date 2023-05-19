abstract class Expr
{
	internal abstract R accept<R>(Visitor<R> visitor);

	internal interface Visitor<R>
	{
		R visitAssignExpr(Assign expr);
		R visitBinaryExpr(Binary expr);
		R visitCallExpr(Call expr);
		R visitGroupingExpr(Grouping expr);
		R visitLiteralExpr(Literal expr);
		R visitLogicalExpr(Logical expr);
		R visitUnaryExpr(Unary expr);
		R visitVariableExpr(Variable expr);
	}

	internal class Assign : Expr
	{
		internal readonly Token name;
		internal readonly Expr value;

		internal Assign(Token name, Expr value)
		{
			this.name = name;
			this.value = value;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitAssignExpr(this);
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

	internal class Call : Expr
	{
		internal readonly Expr callee;
		internal readonly Token paren;
		internal readonly List<Expr> arguments;

		internal Call(Expr callee, Token paren, List<Expr> arguments)
		{
			this.callee = callee;
			this.paren = paren;
			this.arguments = arguments;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitCallExpr(this);
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

	internal class Logical : Expr
	{
		internal readonly Expr left;
		internal readonly Token op;
		internal readonly Expr right;

		internal Logical(Expr left, Token op, Expr right)
		{
			this.left = left;
			this.op = op;
			this.right = right;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitLogicalExpr(this);
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

}
