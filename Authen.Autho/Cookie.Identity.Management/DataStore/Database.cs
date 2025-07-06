using System.Text.Json;

namespace Cookie.Identity.Management.DataStore
{
    public class Database
    {
        private static string HashUserName(string userName)
        {
            //bytes
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(userName);
            //hash
            byte[] hash = System.Security.Cryptography.MD5.HashData(bytes);
            //convert to base64 string
            return Convert.ToBase64String(hash);

        }

        public static async Task StoreUserInFileAsync(User user)
        {
            var fileName = HashUserName(user.UserName);
            await using var fileStreamWriter = File.OpenWrite(fileName);
            await JsonSerializer.SerializeAsync(fileStreamWriter, user);


        }
        public static async Task<User?> GetUserFromFile(string userName)
        {
            var fileName = HashUserName(userName);
            
            await using var fileStreamReader = File.OpenRead(fileName);
           return await JsonSerializer.DeserializeAsync<User>(fileStreamReader);
        }


    }
    public sealed class User
    {
        public required string UserName { get; set; }
        public  string PasswordHash { get; set; } = string.Empty;
        public  List<UserClaims> Claims { get; set; } = new List<UserClaims>();

    }
    public sealed class UserClaims
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
    }
}
