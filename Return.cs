class Return : RuntimeException
{
	readonly internal object? value;

	internal Return(object? value) : base(token: null, message: null)
	{
		this.value = value;
	}
}