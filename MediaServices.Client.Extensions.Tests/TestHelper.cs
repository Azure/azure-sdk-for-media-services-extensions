using System.Configuration;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaServices.Client.Extensions.Tests
{
    public class TestHelper
    {
        public static CloudMediaContext CreateContext()
        {
            return new CloudMediaContext(
                ConfigurationManager.AppSettings["MediaServiceAccountName"],
                ConfigurationManager.AppSettings["MediaServiceAccountKey"]);
        }
    }
}