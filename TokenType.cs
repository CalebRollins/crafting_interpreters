enum TokenType
{
	// Single-character tokens
	LeftParen, RightParen, LeftBrace, RightBrace, Comma, Dot, Minus, Plus, Semicolon, Slash, Star, Question, Colon,

	// One or two character tokens
	Bang, BangEqual, Equal, EqualEqual, Greater, GreaterEqual, Less, LessEqual,

	// Literals
	Identifier, Str, Num,

	// Keywords
	And, Class, Else, False, Fun, For, If, Nil, Or, Print, Ret, Super, This, True, Var, While, EOF
}

