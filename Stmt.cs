abstract class Stmt
{
	internal abstract R accept<R>(Visitor<R> visitor);

	internal interface Visitor<R>
	{
		R visitBlockStmt(Block stmt);
		R visitExpressionStmt(Expression stmt);
		R visitVarStmt(Var stmt);
		R visitPrintStmt(Print stmt);
	}

	internal class Block : Stmt
	{
		internal readonly List<Stmt> statements;

		internal Block(List<Stmt> statements)
		{
			this.statements = statements;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitBlockStmt(this);
	}

	internal class Expression : Stmt
	{
		internal readonly Expr expression;

		internal Expression(Expr expression)
		{
			this.expression = expression;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitExpressionStmt(this);
	}

	internal class Var : Stmt
	{
		internal readonly Token name;
		internal readonly Expr? initializer;

		internal Var(Token name, Expr? initializer)
		{
			this.name = name;
			this.initializer = initializer;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitVarStmt(this);
	}

	internal class Print : Stmt
	{
		internal readonly Expr expression;

		internal Print(Expr expression)
		{
			this.expression = expression;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitPrintStmt(this);
	}

}
