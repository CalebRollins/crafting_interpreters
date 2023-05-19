
class Clock : LoxCallable
{
	public int arity() => 0;

	public object call(Interpreter interpreter, List<object?> arguments) =>
		(double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

	public override string ToString() => "<native fn>";
}