class RuntimeException : SystemException
{
	internal readonly Token? token;

	internal RuntimeException(Token? token, string? message) : base(message)
	{
		this.token = token;
	}
}