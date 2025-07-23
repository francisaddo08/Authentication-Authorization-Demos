namespace Basic.Authen.Demo.Domain
{
    internal sealed class User
    {
        public User( string userName, string password)
        {
            Username = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));

        }
        public  string Username { get; init; }
        public  string Password { get; init; } 
        public string PasswordHash { get; set; } = string.Empty;
    }
}
