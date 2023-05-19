using static TokenType;
class Parser
{
	private readonly List<Token> tokens;
	private int current = 0;

	public Parser(List<Token> tokens)
	{
		this.tokens = tokens;
	}

	private Token peek() => tokens[current];

	private Token previous() => tokens[current - 1];

	private bool isAtEnd() => peek().type == EOF;

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

	private class ParseExpection : Exception { }
	private ParseExpection error(Token token, string message)
	{
		Error.error(token, message);
		return new ParseExpection();
	}

	/// <summary>
	/// Advances if the current token is of type `type`,
	/// otherwise throws an error with `message`
	/// </summary>
	private Token consume(TokenType type, string message)
	{
		if (check(type)) return advance();
		throw error(peek(), message);
	}

	/// <summary>
	/// Advances the Parser to a point where it can continue parsing after a panic
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
			// We already successfully showed the first error and we're avoiding cascading errors on a best-effort basis.
			if (previous().type == Semicolon) return;

			// Tokens we would expect to see at the beginning of a statement
			var statementBeginnings = new TokenType[] { Class, For, Fun, If, Print, Ret, Var, While };
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

	private Expr expression() => assignment();

	private Expr assignment()
	{
		Expr expr = ternary();

		if (match(Equal))
		{
			Token equals = previous();
			Expr value = assignment();

			if (expr is Expr.Variable)
			{
				Token name = ((Expr.Variable)expr).name;
				return new Expr.Assign(name, value);
			}
			error(equals, "Invalid assignment target.");
		}

		return expr;
	}

	// ternary -> equality "?" ternary ":" ternary
	//          | equality 
	private Expr ternary()
	{
		// TODO: Does the following expression make sense? Maybe it is syntatically correct, but non-booleans will always evaluate to false?
		// "lorem ipsum" ? true : false 
		// Will probably revisit once we get to conditionals. If you remove this, remove the ? and : tokentypes 
		Expr expr = equality();
		if (match(Question))
		{
			expr = new Expr.Binary(expr, previous(), ternary());
			consume(Colon, "Expected ':' for ternary expression.");
			return new Expr.Binary(expr, previous(), ternary());
		}
		else return expr;
	}
	private Expr equality() => leftAssociative(comparison, BangEqual, EqualEqual);
	private Expr comparison() => leftAssociative(term, Greater, GreaterEqual, Less, LessEqual);
	private Expr term() => leftAssociative(factor, Minus, Plus);
	private Expr factor() => leftAssociative(unary, Slash, Star);
	private Expr unary()
	{
		if (match(Bang, Minus))
			return new Expr.Unary(op: previous(), right: unary());
		else
			return call();
	}

	private Expr call()
	{
		Expr expr = primary();

		while (true)
		{
			if (match(LeftParen))
				expr = finishCall(expr);
			else
				break;
		}

		return expr;
	}

	/// <summary>
	/// Parses the argument list
	/// </summary>
	private Expr finishCall(Expr callee)
	{
		var arguments = new List<Expr>();
		if (!check(RightParen))
		{
			do
			{
				if (arguments.Count() >= 255)
					error(peek(), "Can't have more than 255 arguments.");

				arguments.Add(expression());
			} while (match(Comma));
		}

		Token paren = consume(RightParen, "Expect ')' after arguments.");

		return new Expr.Call(callee, paren, arguments);
	}

	private Expr primary()
	{
		if (match(False)) return new Expr.Literal(false);
		if (match(True)) return new Expr.Literal(true);
		if (match(Nil)) return new Expr.Literal(null);
		if (match(Num, Str)) return new Expr.Literal(previous().literal);
		if (match(Identifier)) return new Expr.Variable(previous());
		if (match(LeftParen))
		{
			Expr expr = expression();
			consume(RightParen, "Expect ')' after expression.");
			return new Expr.Grouping(expr);
		}

		throw error(peek(), "Expect expression.");
	}

	private Stmt printStatement()
	{
		Expr value = expression();
		consume(Semicolon, "Expect ';' after value.");
		return new Stmt.Print(value);
	}

	private Stmt expressionStatement()
	{
		Expr value = expression();
		consume(Semicolon, "Expect ';' after expression.");
		return new Stmt.Expression(value);
	}

	private List<Stmt> block()
	{
		var statements = new List<Stmt>();
		while (!check(RightBrace) && !isAtEnd())
		{
			var decl = declaration();
			if (decl is not null)
				statements.Add(decl);
		}

		consume(RightBrace, "Expect '}' after block.");
		return statements;
	}

	private Stmt ifStatement()
	{
		// "if" "(" expression ")" statement
		consume(LeftParen, "Expect '(' after 'if'.");
		Expr condition = expression();
		consume(RightParen, "Expect ')' after if condition.");
		Stmt thenBranch = statement();

		// ("else" statement)? ;
		Stmt? elseBranch = null;
		if (match(Else))
			elseBranch = statement();

		return new Stmt.If(condition, thenBranch, elseBranch);
	}

	private Stmt statement()
	{
		if (match(For)) return forStatement();
		if (match(While)) return whileStatment();
		if (match(If)) return ifStatement();
		if (match(Print)) return printStatement();
		if (match(Ret)) return returnStatement();
		if (match(LeftBrace)) return new Stmt.Block(block());

		return expressionStatement();
	}

	private Stmt returnStatement()
	{
		Token keyword = previous();
		Expr? value = null;
		if (!check(Semicolon))
			value = expression();

		consume(Semicolon, "Expect ';' after return value.");
		return new Stmt.Return(keyword, value);
	}

	private Stmt forStatement()
	{
		consume(LeftParen, "Expect '(' after 'for'.");

		// ( varDecl | exprStmt | ";" )
		Stmt? initializer;
		if (match(Semicolon))
			initializer = null;
		else if (match(Var))
			initializer = varDeclaration();
		else
			initializer = expressionStatement();

		// expression? ";"
		Expr? condition = null;
		if (!check(Semicolon))
			condition = expression();

		consume(Semicolon, "Expect ';' after loop condition.");

		// expression? ";"
		Expr? increment = null;
		if (!check(RightParen))
			increment = expression();

		consume(RightParen, "Expect ')' after for clauses.");

		Stmt body = statement();

		// For loops are syntactic sugar. Here we do the desugaring.
		if (increment is not null)
		{
			// Execute increment after every execution of body
			var statements = new List<Stmt> { body, new Stmt.Expression(increment) };
			body = new Stmt.Block(statements);
		}

		// Execute the loop while condition is true
		if (condition is null)
			condition = new Expr.Literal(true);
		body = new Stmt.While(condition, body);

		if (initializer is not null)
		{
			// Execute initializer once before body
			var statements = new List<Stmt> { initializer, body };
			body = new Stmt.Block(statements);
		}

		return body;
	}

	private Stmt whileStatment()
	{
		consume(LeftParen, "Expect '(' after 'while'.");
		Expr condition = expression();
		consume(RightParen, "Expect ')' after while condition.");
		Stmt body = statement();

		return new Stmt.While(condition, body);
	}

	private Stmt? declaration()
	{
		try
		{
			if (match(Fun)) return function("function");
			if (match(Var)) return varDeclaration();
			return statement();
		}
		catch (ParseExpection ex)
		{
			synchronize();
			return null;
		}
	}

	private Stmt? function(string kind)
	{
		Token name = consume(Identifier, $"Expect {kind} name.");
		consume(LeftParen, $"Expect '(' after {kind} name.");
		var parameters = new List<Token>();
		if (!check(RightParen))
		{
			do
			{
				if (parameters.Count() >= 255)
					error(peek(), "Can't have more than 255 parameters.");
				parameters.Add(consume(Identifier, "Expect parameter name."));
			} while (match(Comma));
		}
		consume(RightParen, $"Expect ')' after {kind} name.");
		consume(LeftBrace, $"Expect '{{' before {kind} body.");
		List<Stmt> body = block();
		return new Stmt.Function(name, parameters, body);
	}

	private Stmt? varDeclaration()
	{
		Token name = consume(Identifier, "Expect variable name.");
		Expr? initializer = null;
		if (match(Equal))
			initializer = expression();
		consume(Semicolon, "Expect ';' after variable declaration.");
		return new Stmt.Var(name, initializer);
	}

	public List<Stmt> parse()
	{
		var statements = new List<Stmt>();

		while (!isAtEnd())
		{
			var decl = declaration();
			if (decl is not null)
				statements.Add(decl);
		}

		return statements;
	}
}