using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var credential = new AzureCliCredential();

var client = new SecretClient(
    new Uri("https://kvteched2026.vault.azure.net/"),
    credential);

var secret = await client.GetSecretAsync("Gopas-TE2026-DB");

if (string.IsNullOrEmpty(secret.Value.Value))
{
    Console.WriteLine("Secret value is empty or null.");
}
else
{
        Console.WriteLine(secret.Value.Value);
}