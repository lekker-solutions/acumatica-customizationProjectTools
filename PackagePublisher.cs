#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------
#endregion

using System.ServiceModel;

namespace Velixo.Common.CustomizationPackageTools
{
    public class PackagePublisher
    {
        public static async Task PublishCustomizationPackage(string packageFilename, string packageName, string url, string username, string password)
        {
            BasicHttpBinding binding = new BasicHttpBinding() {  AllowCookies = true };
            binding.Security.Mode  = BasicHttpSecurityMode.Transport;
            binding.OpenTimeout    = new TimeSpan(0, 10, 0);
            binding.SendTimeout    = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            
            EndpointAddress address = new EndpointAddress(url + "/api/ServiceGate.asmx");
            
            var gate = new ServiceGate.ServiceGateSoapClient(binding, address);

            Console.WriteLine($"Logging in to {url}...");
            await gate.LoginAsync(username, password);
            Console.WriteLine($"Uploading package...");
            await gate.UploadPackageAsync(packageName, File.ReadAllBytes(packageFilename), true);
            Console.WriteLine($"Publishing customizations...");
            await gate.PublishPackagesAsync(new string[] { packageName }, true);
            Console.WriteLine($"Logging out...");
            await gate.LogoutAsync();
        }

        public const string LoginUrl        = "/entity/auth/login";
        public const string LogoutUrl       = "/entity/auth/logout";
        public const string ImportUrl       = "/CustomizationApi/Import";
        public const string BeginPublishUrl = "/CustomizationApi/BeginPublish";
        public const string EndPublishUrl = "/CustomizationApi/EndPublish";
        

        public static async Task PublishCustomizationPackageREST(string packageFilename, string packageName, string url,
                                                             string username,        string password)
        {

        }
    }
}