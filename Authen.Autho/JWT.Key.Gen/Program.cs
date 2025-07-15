internal class Program
{
    private static void Main(string[] args)
    {
        var key = System.Security.Cryptography.RSA.Create();
        var privateKey = key.ExportRSAPrivateKey();
        File.WriteAllBytes("privateKey", privateKey);
    }
}