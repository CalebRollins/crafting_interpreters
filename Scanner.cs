using static TokenType;
class Scanner
{
	private readonly string source;
	private readonly List<Token> tokens = new List<Token>();

	private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
	{
		["and"] = And,
		["class"] = Class,
		["else"] = Else,
		["false"] = False,
		["for"] = For,
		["fun"] = Fun,
		["if"] = If,
		["nil"] = Nil,
		["or"] = Or,
		["print"] = Print,
		["return"] = Return,
		["super"] = Super,
		["this"] = This,
		["true"] = True,
		["var"] = Var,
		["while"] = While
	};

	/// <summary>
	/// The start of the lexeme being scanned
	/// </summary>
	private int start = 0;

	/// <summary>
	/// The character currently being considered
	/// </summary>
	private int current = 0;
	private int line = 1;

	public Scanner(string source)
	{
		this.source = source;
	}

	public List<Token> scanTokens()
	{
		while (!isAtEnd())
		{
			// We are at the beginning of the next lexeme
			start = current;
			scanToken();
		}
		tokens.Add(new Token(EOF, "", null, line));
		return tokens;
	}


	private void scanToken()
	{
		char c = advance();
		switch (c)
		{
			case '(': addToken(LeftParen); break;
			case ')': addToken(RightParen); break;
			case '{': addToken(LeftBrace); break;
			case '}': addToken(RightBrace); break;
			case ',': addToken(Comma); break;
			case '.': addToken(Dot); break;
			case '-': addToken(Minus); break;
			case '+': addToken(Plus); break;
			case ';': addToken(Semicolon); break;
			case '*': addToken(Star); break;
			case '!': addToken(match('=') ? BangEqual : Bang); break;
			case '=': addToken(match('=') ? EqualEqual : Equal); break;
			case '<': addToken(match('=') ? LessEqual : Less); break;
			case '>': addToken(match('=') ? GreaterEqual : Greater); break;
			case '/':
				if (match('/'))
					// A comment goes until the end of the line
					while (peek() != '\n' && !isAtEnd()) advance();
				else if (match('*'))
					multilineComment();
				else
					addToken(Slash);
				break;
			case ' ': case '\r': case '\t': break;
			case '\n':
				line++;
				break;
			case '"': str(); break;
			default:
				if (isDigit(c))
					num();
				else if (isAlpha(c))
					identifier();
				else
					Error.error(line, $"Unexpected character {c}");
				break;
		}
	}

	private void multilineComment()
	{
		// How many unclosed /*'s have we encountered?
		int unclosedCount = 1;

		while (!isAtEnd())
		{
			if (match('/') && match('*'))
				unclosedCount++;
			else if (match('*') && match('/'))
			{
				unclosedCount--;
				if (unclosedCount == 0)
					break;
			}
			else if (match('\n'))
				line++;
			else
				advance();
		}

		if (unclosedCount > 0)
			Error.error(line, "Unclosed multi-line comment");
	}

	private bool isAlphaNumeric(char c) => isAlpha(c) || isDigit(c);

	private bool isAlpha(char c)
	{
		return
			(c >= 'a' && c <= 'z') ||
			(c >= 'A' && c <= 'Z') ||
			c == '_';
	}

	private void identifier()
	{
		while (isAlphaNumeric(peek())) advance();

		string text = source[start..current];
		if (!keywords.TryGetValue(text, out var type))
			type = Identifier;
		addToken(type);
	}

	private bool isDigit(char c) => c >= '0' && c <= '9';

	private void num()
	{
		while (isDigit(peek())) advance();

		if (peek() == '.' && isDigit(peekNext()))
		{
			// Consume the .
			advance();
			while (isDigit(peek())) advance();
		}

		addToken(Num, Double.Parse(source[start..current]));
	}

	private void str()
	{
		while (peek() != '"' && !isAtEnd())
		{
			// We allow multi-line strings.
			// So if we hit a new line then we need to advance to the next line.
			// TODO: Should handle both kinds of new-lines?
			if (peek() == '\n') line++;
			advance();
		}

		if (isAtEnd())
		{
			Error.error(line, "Unterminated string.");
			return;
		}

		// Consume the closing "
		advance();

		string value = source[(start + 1)..(current - 1)];
		addToken(Str, value);
	}

	private char peek()
	{
		if (isAtEnd()) return '\0';
		return source[current];
	}

	private char peekNext()
	{
		if (current + 1 > source.Length) return '\0';
		return source[current + 1];
	}

	/// <summary>
	/// Like a conditional advance() - only advances when the next character is expected
	/// </summary>
	private bool match(char expected)
	{
		if (isAtEnd()) return false;
		if (source[current] != expected) return false;

		current++;
		return true;
	}

	private char advance() => source[current++];

	private bool isAtEnd() => current >= source.Length;

	private void addToken(TokenType type) => addToken(type, null);

	private void addToken(TokenType type, object? literal)
	{
		string text = source[start..current];
		tokens.Add(new Token(type, text, literal, line));
	}

}