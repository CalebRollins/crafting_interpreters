namespace Lx
{
	class Environment
	{
		private readonly Dictionary<string, object> values = new Dictionary<string, object>();

		internal void define(string name, object value) => values.Add(name, value);

		internal object get(Token name)
		{
			if (values.ContainsKey(name.lexeme))
				return values[name.lexeme];

			throw new RuntimeException(name, $"Undefined variable {name.lexeme}.");
		}
	}
}