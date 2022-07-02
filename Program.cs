using System.Text;

class Lox
{
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
		// TODO: correct encoding?
		run(Encoding.UTF8.GetString(bytes));

		if (Error.HadError) Environment.Exit(65);
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
		Expr? expression = parser.parse();

		if (Error.HadError || expression == null) return;

		Console.WriteLine(new AstPrinter().print(expression));
	}
}