using System;
using System.Configuration;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace MediaServices.Client.Extensions.Tests
{
    public class TestHelper
    {
        public static CloudMediaContext CreateContext()
        {
            return new CloudMediaContext(
                new Uri(ConfigurationManager.AppSettings["MediaServicesUri"]),
                ConfigurationManager.AppSettings["MediaServiceAccountName"],
                ConfigurationManager.AppSettings["MediaServiceAccountKey"],
                ConfigurationManager.AppSettings["MediaServicesAccessScope"],
                ConfigurationManager.AppSettings["MediaServicesAcsBaseAddress"]
                );
        }
    }
}