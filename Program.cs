using System.Text;

class Lox
{
	private static readonly Interpreter interpreter = new Interpreter();

	static void Main(string[] args)
	{
		if (args.Length > 1)
		{
			Console.Error.WriteLine("Usage: jlox [script]");
			// Exit codes are as defined in the UNIX "sysexits.h" header
			Environment.Exit(64);
		}
		else if (args.Length == 1)
		{
			runFile(args[0]);
		}
		else
		{
			runPrompt();
		}
	}

	private static void runFile(string path)
	{
		byte[] bytes = File.ReadAllBytes(path);
		run(Encoding.UTF8.GetString(bytes));
		if (Error.HadError) Environment.Exit(65);
		if (Error.HadRuntimeError) Environment.Exit(70);
	}

	private static void runPrompt()
	{
		while (true)
		{
			Console.Write("> ");
			var line = Console.ReadLine();
			if (line == null) break;
			run(line);
			Error.HadError = false;
		}
	}

	private static void run(string source)
	{
		var scanner = new Scanner(source);
		List<Token> tokens = scanner.scanTokens();

		var parser = new Parser(tokens);
		var statements = parser.parse();

		// Stop if there was a syntax error
		if (Error.HadError) return;

		interpreter.interpret(statements);
	}
}