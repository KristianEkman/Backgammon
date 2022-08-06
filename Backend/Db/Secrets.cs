using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System.IO;
using Microsoft.Extensions.Configuration;

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

        internal static string FbAppToken(IConfiguration config)
        {
#if DEBUG
            return File.ReadAllText("fbapptoken.txt");
#endif
            return config.GetValue<string>("FbAppToken");
        }
    }
}
