using System.Diagnostics.CodeAnalysis;
using static TokenType;

class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?> // Can't use void as type parameter in C#
{
	private Lx.Environment environment = new Lx.Environment();
	private object? evaluate(Expr expr) => expr.accept(this);

	private void execute(Stmt statement) => statement.accept(this);

	internal void interpret(List<Stmt> statements)
	{
		try
		{
			foreach (var statement in statements)
				execute(statement);
		}
		catch (RuntimeException ex)
		{
			Error.runtimeError(ex);
		}
	}

	private string stringify(object? obj)
	{
		if (obj is null) return "nil";

		if (obj is double)
		{
			string text = obj.ToString() ?? "";
			if (text.EndsWith(".0"))
				text = text[0..(text.Length - 2)];

			return text;
		}

		return obj.ToString() ?? "";
	}

	public object? visitBinaryExpr(Expr.Binary expr)
	{
		var left = evaluate(expr.left);
		var right = evaluate(expr.right);

		switch (expr.op.type)
		{
			case BangEqual:
				return !isEqual(left, right);
			case EqualEqual:
				return isEqual(left, right);
			case Greater:
				checkNumberOperands(expr.op, left, right);
				return (double)left > (double)right;
			case GreaterEqual:
				checkNumberOperands(expr.op, left, right);
				return (double)left >= (double)right;
			case Less:
				checkNumberOperands(expr.op, left, right);
				return (double)left < (double)right;
			case LessEqual:
				checkNumberOperands(expr.op, left, right);
				return (double)left <= (double)right;
			case Plus:
				if (left is double && right is double)
					return (double)left + (double)right;
				// If either operand is a string then we concatenate them
				if ((left is string && right is not null) || (right is string && left is not null))
					return left.ToString() + right.ToString();
				throw new RuntimeException(expr.op, "Invalid operands for + operation.");
			case Minus:
				checkNumberOperands(expr.op, left, right);
				return (double)left - (double)right;
			case Star:
				checkNumberOperands(expr.op, left, right);
				return (double)left * (double)right;
			case Slash:
				checkNumberOperands(expr.op, left, right);
				if ((double)right == 0)
					throw new RuntimeException(expr.op, "Cannot divide by zero.");
				return (double)left / (double)right;
		}
		// unreachable
		return null;
	}

	private void checkNumberOperands(Token op, [NotNull] object? left, [NotNull] object? right)
	{
		if (left is not double || right is not double)
			throw new RuntimeException(op, "Operands must be numbers.");
	}

	private bool isEqual(object? a, object? b)
	{
		if (a == null && b == null) return true;
		if (a == null) return false;

		return a.Equals(b);
	}

	public object? visitGroupingExpr(Expr.Grouping expr) => evaluate(expr.expression);

	public object? visitLiteralExpr(Expr.Literal expr) => expr.value;

	public object? visitUnaryExpr(Expr.Unary expr)
	{
		object? right = evaluate(expr.right);

		switch (expr.op.type)
		{
			case TokenType.Bang:
				return !isTruthy(right);
			case TokenType.Minus:
				checkNumberOperand(expr.op, right);
				return -(double)right;
		}

		// unreachable
		return null;
	}

	private void checkNumberOperand(Token op, [NotNull] object? operand)
	{
		if (operand is not double)
			throw new RuntimeException(op, "Operand must be a number.");
	}

	private bool isTruthy(object? obj)
	{
		if (obj == null) return false;
		if (obj is bool) return (bool)obj;
		return true;
	}

	public object? visitExpressionStmt(Stmt.Expression stmt)
	{
		throw new NotImplementedException();
	}

	public object? visitPrintStmt(Stmt.Print stmt)
	{
		throw new NotImplementedException();
	}

	object? Stmt.Visitor<object?>.visitExpressionStmt(Stmt.Expression stmt)
	{
		evaluate(stmt.expression);
		return null;
	}

	object? Stmt.Visitor<object?>.visitPrintStmt(Stmt.Print stmt)
	{
		object? value = evaluate(stmt.expression);
		Console.WriteLine(stringify(value));
		return null;
	}

	public object? visitVariableExpr(Expr.Variable expr) => environment.get(expr.name);

	public object? visitVarStmt(Stmt.Var stmt)
	{
		object? value = null;
		if (stmt.initializer != null)
			value = evaluate(stmt.initializer);

		environment.define(stmt.name.lexeme, value);
		return null;
	}

	public object? visitAssignExpr(Expr.Assign expr)
	{
		object? value = evaluate(expr.value);
		environment.assign(expr.name, value);
		return value;
	}

	public object? visitBlockStmt(Stmt.Block stmt)
	{
		executeBlock(stmt.statements, new Lx.Environment(environment));
		return null;
	}

	private void executeBlock(List<Stmt> statements, Lx.Environment environment)
	{
		Lx.Environment? previous = this.environment;
		try
		{
			this.environment = environment;
			foreach (Stmt statement in statements)
				execute(statement);
		}
		finally
		{
			this.environment = previous;
		}
	}
}