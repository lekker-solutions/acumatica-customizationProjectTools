using AcmPackageTools.Service;
using AcuPackageTools.CmdletBase;
using System;
using System.IO;
using System.Management.Automation;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackageUpload")]
    public class Invoke_ServicePackageUploadCmdlet : ServiceCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pn")]
        public string PackageName { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pfn")]
        public string PackageFileName { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("r")]
        public SwitchParameter ReplacePackage { get; set; }

        protected override void ProcessRecord()
        {
            var client = GetClient();
            client.LoginAsync(new LoginRequest(Username, Password));
            client.UploadPackageAsync(new UploadPackageRequest(PackageName, File.ReadAllBytes(PackageFileName), ReplacePackage));
            client.LogoutAsync(new LogoutRequest());
        }

        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}