using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System.IO;

namespace Backend.Db
{
    public class Secrets
    {
        internal static string GetPw()
        {
            //This is for local use only, when secrets are not accessible.
            if (File.Exists("pw.txt"))
                return File.ReadAllText("pw.txt");

            return "";
        }

        internal static string FbAppToken()
        {
#if DEBUG
            return File.ReadAllText("fbapptoken.txt");
#else

            var options = new SecretClientOptions()
            {
                Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 2,
                        Mode = RetryMode.Exponential
                     }
            };
            var client = new SecretClient(new Uri("https://backgammon-keys.vault.azure.net/"), new DefaultAzureCredential(), options);
            KeyVaultSecret secret = client.GetSecret("bgfbat");
            string secretValue = secret.Value;
            return secretValue;
#endif
        }
    }
}
