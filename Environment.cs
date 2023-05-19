namespace Lx
{
	class Environment
	{
		private readonly Dictionary<string, object?> values = new Dictionary<string, object?>();
		internal readonly Environment? enclosing;

		internal Environment(Environment? enclosing = null)
		{
			this.enclosing = enclosing;
		}

		/// <summary>
		/// Defines a new variable in the current environment
		/// </summary>
		internal void define(string name, object? value) => values.Add(name, value);

		internal object? get(Token name)
		{
			if (values.ContainsKey(name.lexeme))
			{
				if (values[name.lexeme] == null)
					throw new RuntimeException(name, $"Unassigned variable '{name.lexeme}'.");

				return values[name.lexeme];
			}

			if (enclosing is not null) return enclosing.get(name);

			throw new RuntimeException(name, $"Undefined variable '{name.lexeme}'.");
		}

		internal void assign(Token name, object? value)
		{
			if (values.ContainsKey(name.lexeme))
			{
				values[name.lexeme] = value;
				return;
			}

			if (enclosing is not null)
			{
				enclosing.assign(name, value);
				return;
			}

			throw new RuntimeException(name, $"Attempted to assign to undefined variable '{name.lexeme}'.");
		}
	}
}