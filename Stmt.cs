abstract class Stmt
{
	internal abstract R accept<R>(Visitor<R> visitor);

	internal interface Visitor<R>
	{
		R visitBlockStmt(Block stmt);
		R visitExpressionStmt(Expression stmt);
		R visitFunctionStmt(Function stmt);
		R visitIfStmt(If stmt);
		R visitVarStmt(Var stmt);
		R visitPrintStmt(Print stmt);
		R visitReturnStmt(Return stmt);
		R visitWhileStmt(While stmt);
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

	internal class Function : Stmt
	{
		internal readonly Token name;
		internal readonly List<Token> parameters;
		internal readonly List<Stmt> body;

		internal Function(Token name, List<Token> parameters, List<Stmt> body)
		{
			this.name = name;
			this.parameters = parameters;
			this.body = body;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitFunctionStmt(this);
	}

	internal class If : Stmt
	{
		internal readonly Expr condition;
		internal readonly Stmt thenBranch;
		internal readonly Stmt? elseBranch;

		internal If(Expr condition, Stmt thenBranch, Stmt? elseBranch)
		{
			this.condition = condition;
			this.thenBranch = thenBranch;
			this.elseBranch = elseBranch;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitIfStmt(this);
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

	internal class Return : Stmt
	{
		internal readonly Token keyword;
		internal readonly Expr? value;

		internal Return(Token keyword, Expr? value)
		{
			this.keyword = keyword;
			this.value = value;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitReturnStmt(this);
	}

	internal class While : Stmt
	{
		internal readonly Expr condition;
		internal readonly Stmt body;

		internal While(Expr condition, Stmt body)
		{
			this.condition = condition;
			this.body = body;
		}

		internal override R accept<R>(Visitor<R> visitor) => 
			visitor.visitWhileStmt(this);
	}

}
