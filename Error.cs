/// <summary>
/// Code for error handling and reporting
/// </summary>
class Error
{
	public static bool HadError { get; set; } = false;
	public static bool HadRuntimeError { get; set; } = false;

	public static void error(int line, string message)
	{
		report(line, "", message);
	}

	public static void error(Token token, string message)
	{
		if (token.type == TokenType.EOF)
			report(token.line, " at end", message);
		else
			report(token.line, $" at '{token.lexeme}'", message);
	}

	internal static void runtimeError(RuntimeException ex)
	{
		Console.Error.WriteLine($"{ex.Message}\n[line {ex.token.line}]");
		HadRuntimeError = true;
	}

	private static void report(int line, string where, string message)
	{
		Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
		HadError = true;
	}
}