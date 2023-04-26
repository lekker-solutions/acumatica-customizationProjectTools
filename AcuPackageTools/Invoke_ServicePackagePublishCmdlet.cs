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
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias("m")]
        public SwitchParameter MergeWithExisting { get; set; }

        internal override void PerformOperations(ServiceGateSoap client)
        {
            client.PublishPackages(new PublishPackagesRequest(PackageNames, MergeWithExisting));
        }
    }
}