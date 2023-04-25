#region #Copyright
//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using AcmPackageTools.Service;

namespace AcuPackageTools
{
    public class PackagePublisher
    {
        public static void PublishCustomizationPackage(string packageFilename, string packageName, string url, string username, string password, bool replaceIfPackageExists = true)
        {
            BasicHttpBinding binding = new BasicHttpBinding() {  AllowCookies = true };
            binding.Security.Mode  = BasicHttpSecurityMode.Transport;
            binding.OpenTimeout    = new TimeSpan(0, 10, 0);
            binding.SendTimeout    = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            
            EndpointAddress address = new EndpointAddress(url + "/api/ServiceGate.asmx");
            
            var gate = new ServiceGateSoapClient(binding, address);

            Console.WriteLine($"Logging in to {url}...");
            gate.LoginAsync(new LoginRequest(username, password));
            Console.WriteLine($"Uploading package...");
            gate.UploadPackageAsync(new UploadPackageRequest(packageName, File.ReadAllBytes(packageFilename), replaceIfPackageExists));
            Console.WriteLine($"Publishing customizations...");
            gate.PublishPackages(new PublishPackagesRequest(new [] { packageName }, true));
            Console.WriteLine($"Logging out...");
            gate.LogoutAsync(new LogoutRequest());
        }

        public const string LoginUrl        = "/entity/auth/login";
        public const string LogoutUrl       = "/entity/auth/logout";
        public const string ImportUrl       = "/CustomizationApi/Import";
        public const string BeginPublishUrl = "/CustomizationApi/BeginPublish";
        public const string EndPublishUrl = "/CustomizationApi/EndPublish";
        

        public static void PublishCustomizationPackageREST(string packageFilename, string packageName, string url,
                                                             string username,        string password)
        {

        }
    }
}