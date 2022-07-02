using static TokenType;
class Parser
{
	private readonly List<Token> tokens;
	private int current = 0;

	public Parser(List<Token> tokens)
	{
		this.tokens = tokens;
	}

	private bool isAtEnd() => peek().type == EOF;

	private Token peek() => tokens[current];

	private Token previous() => tokens[current - 1];

	private bool check(TokenType type)
	{
		if (isAtEnd()) return false;
		return peek().type == type;
	}

	private Token advance()
	{
		if (!isAtEnd()) current++;
		return previous();
	}

	/// <summary>
	/// Is the current token one of the types in types? 
	/// If yes, consumes the token and returns true.
	/// If no, returns false.
	/// </summary>
	private bool match(params TokenType[] types)
	{
		foreach (TokenType type in types)
		{
			if (check(type))
			{
				advance();
				return true;
			}
		}
		return false;
	}

	private Token consume(TokenType type, string message)
	{
		if (check(type)) return advance();
		throw error(peek(), message);
	}

	private class ParseExpection : Exception { }
	private ParseExpection error(Token token, string message)
	{
		Error.error(token, message);
		return new ParseExpection();
	}

	/// <summary>
	/// Gets the parser back into a state where it can continue parsing after a panic
	/// </summary>
	private void synchronize()
	{
		advance();
		// Throw away tokens until we get to a new statement.
		// We may miss errors in the tokens we throw away,
		// but the tradeoff here is that we will avoid showing a bunch of cascading errors 
		// that are really just the result of one error.
		while (!isAtEnd())
		{
			// Semicolons within a for loop may be a false positive here, but that's ok.
			// We already successfully showed the first error and we're avoiding cascading errors is done on a best-effort basis.
			if (previous().type == Semicolon) return;

			// Tokens we would expect to see at the beginning of a statement
			var statementBeginnings = new TokenType[] { Class, For, Fun, If, Print, Return, Var, While };
			if (statementBeginnings.Contains(peek().type))
				return;

			advance();
		}

	}

	private Expr leftAssociative(Func<Expr> op, params TokenType[] types)
	{
		Expr expr = op();

		while (match(types))
			expr = new Expr.Binary(left: expr, op: previous(), right: op());

		return expr;
	}

	private Expr expression() => equality();
	private Expr equality() => leftAssociative(comparison, BangEqual, EqualEqual);
	private Expr comparison() => leftAssociative(term, Greater, GreaterEqual, Less, LessEqual);
	private Expr term() => leftAssociative(factor, Minus, Plus);
	private Expr factor() => leftAssociative(unary, Slash, Star);
	private Expr unary()
	{
		if (match(Bang, Minus))
			return new Expr.Unary(op: previous(), right: unary());
		else
			return primary();
	}
	private Expr primary()
	{
		if (match(False)) return new Expr.Literal(false);
		if (match(True)) return new Expr.Literal(true);
		if (match(Nil)) return new Expr.Literal(null);
		if (match(Num, Str)) return new Expr.Literal(previous().literal);
		if (match(LeftParen))
		{
			Expr expr = expression();
			consume(RightParen, "Expect ')' after expression.");
			return new Expr.Grouping(expr);
		}

		throw error(peek(), "Expect expression.");
	}

	public Expr? parse()
	{
		try
		{
			return expression();
		}
		catch (ParseExpection ex)
		{
			var x = ex; // shutup, roslyn
			return null;
		}
	}
}