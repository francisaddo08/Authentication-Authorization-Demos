namespace Basic.Authen.Demo.Dtos
{
    internal sealed class RegisterDto
    {
        public required string Username { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}
