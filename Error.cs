/// <summary>
/// Code for error handling and reporting
/// </summary>
class Error
{
	public static Boolean HadError { get; set; } = false;

	public static void error(int line, string message)
	{
		report(line, "", message);
	}

	private static void report(int line, string where, string message)
	{
		Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
		HadError = true;
	}
}