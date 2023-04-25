using System;
using System.Management.Automation;
using System.ServiceModel.Channels;
using System.ServiceModel;
using AcmPackageTools.Service;
using AcuPackageTools.CmdletBase;

namespace AcuPackageTools
{
    [Cmdlet(VerbsLifecycle.Invoke, "ServicePackagePublish")]
    public class Invoke_ServicePackagePublishCmdlet : ServiceCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("pn")]
        public string[] PackageNames { get; set; }

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
        public SwitchParameter MergeWithExisting { get; set; }

        protected override void ProcessRecord()
        {
            var client = GetClient();
            client.Login(new LoginRequest(Username, Password));
            client.PublishPackages(new PublishPackagesRequest(PackageNames, MergeWithExisting));
            client.Logout(new LogoutRequest());
        }

        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}