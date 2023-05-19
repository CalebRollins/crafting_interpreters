class LoxFunction : LoxCallable
{
	private readonly Stmt.Function declaration;
	private readonly Lx.Environment closure;

	internal LoxFunction(Stmt.Function declaration, Lx.Environment closure)
	{
		this.closure = closure;
		this.declaration = declaration;
	}

	public int arity() => declaration.parameters.Count();

	public object? call(Interpreter interpreter, List<object?> arguments)
	{
		var environment = new Lx.Environment(closure);
		for (int i = 0; i < declaration.parameters.Count(); i++)
			environment.define(declaration.parameters[i].lexeme, arguments[i]);

		try
		{
			interpreter.executeBlock(declaration.body, environment);
		}
		catch (Return returnValue)
		{
			return returnValue.value;
		}
		return null;
	}
}